using Domain.Enums;

namespace Application.Interview.DTO;

public record CandidateResponse(
    int Id,
    string FullName,
    string Phone,
    string Email,
    string? Telegram,
    string City,
    string[]? Education,
    string[]? PreviousWork,
    string[] Skills,
    CandidateStatus Status,
    IReadOnlyList<ApplicationProcessResponse> ApplicationProcesses);

public record ApplicationProcessResponse(
    Guid ProcessId,
    int VacancyId,
    string VacancyName,
    IReadOnlyList<InterviewResponse> Interviews);

public record InterviewResponse(
    int Id,
    DateTime Date,
    InterviewStatus Status,
    string? SummaryComment,
    DeciderVerdict? Decision,
    string? VerdictComment);