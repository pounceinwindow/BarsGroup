using MediatR;

namespace Application.Decider.Contracts.Commands;

public record RejectCandidateCommand(
    Guid InterviewId, 
    Guid CandidateId, 
    Guid DeciderId, 
    string RejectionReason
) : IRequest;