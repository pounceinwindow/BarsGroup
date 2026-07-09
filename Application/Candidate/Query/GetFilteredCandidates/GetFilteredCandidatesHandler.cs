using Application.Abstractions.Repositories;
using Application.Interview.DTO;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Candidate.Query.GetFilteredCandidates;

public class GetFilteredCandidatesHandler : IRequestHandler<GetFilteredCandidatesQuery, (List<CandidateDto> Items, int TotalCount)>
{
    private readonly ICandidateRepository _candidates;

    public GetFilteredCandidatesHandler(ICandidateRepository candidates)
    {
        _candidates = candidates;
    }

    public async Task<(List<CandidateDto> Items, int TotalCount)> Handle(GetFilteredCandidatesQuery request, CancellationToken cancellationToken)
    {
        return await _candidates.GetFilteredCandidates(request.Filters);
    }
}
