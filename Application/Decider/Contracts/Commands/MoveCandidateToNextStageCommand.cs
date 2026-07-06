using MediatR;

namespace Application.Decider.Contracts.Commands;

public record MoveCandidateToNextStageCommand(
    Guid InterviewId, 
    Guid CandidateId, 
    int NextStageNumber, 
    Guid DeciderId
) : IRequest;