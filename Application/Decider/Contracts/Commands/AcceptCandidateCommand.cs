using MediatR;

namespace Application.Decider.Contracts.Commands;

public record AcceptCandidateCommand(
    Guid InterviewId, 
    Guid CandidateId, 
    Guid DeciderId, 
    string? OfferComments
) : IRequest;