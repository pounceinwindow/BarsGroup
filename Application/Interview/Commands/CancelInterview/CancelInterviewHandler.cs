using Application.Abstractions;
using Application.Abstractions.Repositories;
using Application.Exceptions;
using Domain.Exceptions;
using MediatR;

namespace Application.Interview.Commands.CancelInterview;

public class CancelInterviewHandler : IRequestHandler<CancelInterviewCommand, int>
{
    private readonly IInterviewRepository _interviews;
    private readonly IUnitOfWork _unitOfWork;

    public CancelInterviewHandler(IInterviewRepository interviews, IUnitOfWork unitOfWork)
    {
        _interviews = interviews;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(CancelInterviewCommand request, CancellationToken cancellationToken)
    {
        var interview = await _interviews.GetByIdAsync(request.InterviewId, cancellationToken)
                        ?? throw new NotFoundException(
                            $"The interview with id {request.InterviewId} was not found.");

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