using Application.Interview.DTO;
using MediatR;

namespace Application.Interview.Queries.GetCandidate;

public record GetCandidateQuery(int Id) : IRequest<CandidateResponse>;