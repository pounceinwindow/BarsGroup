using Application.Candidate.DTO;
using Application.Interview.DTO;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Candidate.Query.GetFilteredCandidates;

public record GetFilteredCandidatesQuery(
    CandidateFilters Filters)
    : IRequest<(List<CandidateListUnit> Items, int TotalCount)>;