using Application.Vacancy;
using Domain.Enums;

namespace Application.Interview.DTO;

public class InterviewFilters
{
    public string Search { get; set; } = string.Empty;
    public InterviewStatus Status { get; set; } = InterviewStatus.All;
    public string Vacancy { get; set; } = string.Empty;
    // Date?

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}