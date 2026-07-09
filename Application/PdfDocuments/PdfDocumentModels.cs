namespace Application.PdfDocuments;

public enum LetterType
{
    Invitation,
    Rejection
}

public sealed record CompetencyScoreModel(
    string Name,
    int Score,
    string? Comment);

public sealed record InterviewProtocolModel(
    string CandidateName,
    string Vacancy,
    DateTime DateUtc,
    string Hr,
    string? Approver,
    string Status,
    string? HrComment,
    string? Decision,
    string? VerdictComment,
    IReadOnlyList<CompetencyScoreModel> Competencies);

public sealed record ProcessInterviewPdfModel(
    DateTime DateUtc,
    string Status);

public sealed record ApplicationProcessPdfModel(
    string VacancyName,
    IReadOnlyList<ProcessInterviewPdfModel> Interviews);

public sealed record CandidateCardModel(
    int Id,
    string FullName,
    string Phone,
    string Email,
    string City,
    string? Telegram,
    string Education,
    string PreviousWork,
    IReadOnlyList<string> Skills,
    string Status,
    IReadOnlyList<ApplicationProcessPdfModel> Processes);

public sealed record ApplicationLetterModel(
    string CandidateName,
    string Vacancy);
