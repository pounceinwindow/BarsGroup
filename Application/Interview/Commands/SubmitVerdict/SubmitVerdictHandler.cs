using Application.Abstractions;
using Application.Abstractions.Repositories;
using Application.Exceptions;
using Domain.Exceptions;
using MediatR;

namespace Application.Interview.Commands.SubmitVerdict;

public class SubmitVerdictHandler : IRequestHandler<SubmitVerdictCommand, int>
{
    private readonly IInterviewRepository _interviews;
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _unitOfWork;

    public SubmitVerdictHandler(
        IInterviewRepository interviews,
        IUserRepository users,
        IUnitOfWork unitOfWork)
    {
        _interviews = interviews;
        _users = users;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(SubmitVerdictCommand request, CancellationToken cancellationToken)
    {
        var interview = await _interviews.GetByIdAsync(request.InterviewId, cancellationToken)
                        ?? throw new NotFoundException(
                            $"The interview with id {request.InterviewId} was not found.");

        if (!await _users.IsDeciderAsync(request.DeciderId, cancellationToken))
        {
            throw new ConflictException("Only a decider can submit a verdict for this interview.");
        }

        if (interview.Verdict is not null)
        {
            throw new ConflictException("Verdict has already been recorded for this interview.");
        }

        try
        {
            var comment = string.IsNullOrWhiteSpace(request.Comment) ? null : request.Comment.Trim();
            var verdict = interview.RecordVerdict(request.DeciderId, request.Decision, comment);
            await _interviews.AddVerdictAsync(verdict, cancellationToken);
        }
        catch (DomainException ex)
        {
            throw new ConflictException(ex.Message);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return interview.Id;
    }
}