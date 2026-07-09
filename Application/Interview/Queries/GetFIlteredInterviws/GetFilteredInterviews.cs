using Application.Abstractions.Repositories;
using Application.Interview.DTO;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interview.Queries.GetFIlteredInterviws;

public record GetFilteredInterviewsQuery(
    InterviewFilters Filters) 
    : IRequest<(List<InterviewDto>, int count)>;

public class GetFilteredInterviewsHandler
    : IRequestHandler<GetFilteredInterviewsQuery, (List<InterviewDto>, int count)>
{
    private readonly IInterviewRepository _interviewRepository;

    public GetFilteredInterviewsHandler(IInterviewRepository interviewRepository)
    {
        _interviewRepository = interviewRepository;
    }

    public Task<(List<InterviewDto>, int count)> Handle(
        GetFilteredInterviewsQuery request, 
        CancellationToken cancellationToken)
    {
        return _interviewRepository.GetFilteredInterviews(
            request.Filters,
            cancellationToken);
    }
}