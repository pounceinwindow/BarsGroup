using Application.Interview.DTO;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Candidate.Query.GetFilteredCandidates;

public record GetFilteredCandidatesQuery(
    ) 
    : IRequest<List<CandidateListUnit>>;

