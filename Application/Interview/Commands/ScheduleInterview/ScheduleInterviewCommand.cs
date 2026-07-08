using MediatR;

namespace Application.Interview.Commands.ScheduleInterview;

public record ScheduleInterviewCommand(
    int CandidateId,
    int VacancyId,
    DateTime Date,
    Guid? ProcessId) : IRequest<int>;