using Application.Abstractions.Repositories;
using Application.Interview.DTO;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Candidate.Query.GetFilteredCandidates;

public class GetFilteredCandidatesHandler : IRequestHandler<GetFilteredCandidatesQuery, List<CandidateListUnit>>
{
    private readonly ICandidateRepository _candidates;
    public GetFilteredCandidatesHandler(ICandidateRepository candidates)
    {
        _candidates = candidates;
    }

    public async Task<List<CandidateListUnit>> Handle(GetFilteredCandidatesQuery request, CancellationToken cancellationToken)
    {
        // [TODO] - тут должна быть фильтрация
        var candidate = await _candidates.GetFilteredCandidates();
        return candidate;
    }
}
