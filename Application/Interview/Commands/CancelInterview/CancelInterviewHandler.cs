using Application.Abstractions;
using Application.Abstractions.Repositories;
using Application.Exceptions;
using Domain.Exceptions;
using MediatR;

namespace Application.Interview.Commands.CancelInterview;

public class CancelInterviewHandler : IRequestHandler<CancelInterviewCommand, int>
{
    private readonly IInterviewRepository _interviews;
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _unitOfWork;

    public CancelInterviewHandler(
        IInterviewRepository interviews,
        IUserRepository users,
        IUnitOfWork unitOfWork)
    {
        _interviews = interviews;
        _users = users;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(CancelInterviewCommand request, CancellationToken cancellationToken)
    {
        var interview = await _interviews.GetByIdAsync(request.InterviewId, cancellationToken)
                        ?? throw new NotFoundException(
                            $"The interview with id {request.InterviewId} was not found.");

        if (interview.HrId != request.HrId
            && !await _users.IsAdminAsync(request.HrId, cancellationToken))
        {
            throw new ConflictException("Only the assigned HR or an administrator can cancel this interview.");
        }

        try
        {
            interview.Cancel();
        }
        catch (InvalidStatusTransitionException ex)
        {
            throw new ConflictException(ex.Message);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return interview.Id;
    }
}
