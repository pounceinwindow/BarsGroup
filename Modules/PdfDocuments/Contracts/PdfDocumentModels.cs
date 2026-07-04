namespace PdfDocuments.Contracts;

public static class DocumentStatuses
{
    public const string New = "New";
    public const string Reviewed = "Reviewed";
    public const string InterviewScheduled = "InterviewScheduled";
    public const string InterviewCompleted = "InterviewCompleted";
    public const string PendingDecision = "PendingDecision";
    public const string Accepted = "Accepted";
    public const string NextStage = "NextStage";
    public const string TalentPool = "TalentPool";
    public const string Rejected = "Rejected";
    public const string Archived = "Archived";
}

public enum LetterType
{
    Invitation,
    Rejection
}

public sealed record CompetencyScoreModel(
    string Name,
    string? Description,
    int Score,
    string? Comment);

public sealed record InterviewProtocolModel(
    int Id,
    string CandidateName,
    string Vacancy,
    DateTime Date,
    string Format,
    string Hr,
    string DecisionMaker,
    string Status,
    string? HrComment,
    string? Decision,
    string? DecisionComment,
    IReadOnlyList<CompetencyScoreModel> Competencies);

public sealed record InterviewSummaryModel(
    int Id,
    DateTime Date,
    string Format,
    string Status,
    double? AverageScore);

public sealed record CandidateApplicationModel(
    int Id,
    string Vacancy,
    string Status,
    DateTime AppliedAt,
    string? HrComment,
    string? Decision,
    string? DecisionComment,
    IReadOnlyList<InterviewSummaryModel> Interviews);

public sealed record CandidateCardModel(
    int Id,
    string FullName,
    string Phone,
    string? Email,
    string City,
    string? Education,
    string? PreviousWork,
    string? Telegram,
    IReadOnlyList<string> Skills,
    DateTime CreatedAt,
    IReadOnlyList<CandidateApplicationModel> Applications);
