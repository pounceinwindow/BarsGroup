using Application.Abstractions.Repositories;
using Application.Exceptions;
using Application.Interview.DTO;
using MediatR;

namespace Application.Interview.Queries.GetCandidate;

public class GetCandidateHandler(ICandidateRepository candidates)
    : IRequestHandler<GetCandidateQuery, CandidateResponse>
{
    public async Task<CandidateResponse> Handle(
        GetCandidateQuery request,
        CancellationToken cancellationToken)
    {
        return await candidates.GetByIdDetailedAsync(request.Id, cancellationToken)
               ?? throw new NotFoundException($"Candidate with id {request.Id} was not found.");
    }
}