using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Models;

public class Interview
{
    // Строки матрицы добавляются только из домена (SubmitProtocol)
    // Приватный список скрывает мутацию; снаружи доступен только read-only вид.
    private readonly List<CompetencyMatrix> _matrixRows = [];

    public int Id { get; private set; }
    public int VacancyId { get; private set; }
    public int CandidateId { get; private set; }

    /// <summary>
    /// Идентификатор отклика, объединяющий этапы интервью кандидата на одну вакансию.
    /// </summary>
    public Guid ProcessId { get; private set; }

    public DateTime Date { get; private set; }
    public InterviewStatus Status { get; private set; }

    /// <summary>
    /// HR, создавший и ведущий интервью.
    /// </summary>
    public int HrId { get; private set; }

    /// <summary>
    /// Общий комментарий HR по итогам интервью.
    /// </summary>
    public string? SummaryComment { get; private set; }

    public Vacancy Vacancy { get; private set; } = null!;
    public Candidate Candidate { get; private set; } = null!;
    public User Hr { get; private set; } = null!;
    public Verdict? Verdict { get; private set; }

    /// <summary>
    /// Строки матрицы компетенций. Заполняются один раз при сабмите протокола HR.
    /// </summary>
    public IReadOnlyCollection<CompetencyMatrix> MatrixRows => _matrixRows.AsReadOnly();

    private Interview()
    {
    }

    /// <summary>
    /// Создаёт первое интервью в рамках нового отклика на вакансию.
    /// </summary>
    public static Interview ScheduleNewProcess(int candidateId, int vacancyId, int hrId, DateTime date)
    {
        return Schedule(candidateId, vacancyId, Guid.NewGuid(), hrId, date);
    }

    /// <summary>
    /// Создаёт интервью следующего этапа в рамках существующего отклика.
    /// </summary>
    public static Interview ScheduleNextStage(
        int candidateId,
        int vacancyId,
        Guid processId,
        int hrId,
        DateTime date)
    {
        if (processId == Guid.Empty)
            throw new DomainException("ProcessId cannot be empty.");

        return Schedule(candidateId, vacancyId, processId, hrId, date);
    }

    /// <summary>
    /// Сохраняет протокол HR: создаёт строки матрицы и переводит интервью в ожидание решения.
    /// </summary>
    public void SubmitProtocol(
        int hrId,
        string? summaryComment,
        IReadOnlyList<ProtocolCompetencyScore> scores)
    {
        EnsureAssignedHr(hrId);
        EnsureStatus(InterviewStatus.Scheduled);

        if (_matrixRows.Count > 0)
            throw new DomainException("Protocol has already been submitted.");

        if (scores.Count == 0)
            throw new DomainException("Protocol must contain at least one competency score.");

        foreach (var competencyScore in scores)
        {
            if (competencyScore.Score is < CompetencyMatrix.NotEvaluatedScore or > CompetencyMatrix.MaxScore)
                throw new ArgumentOutOfRangeException(
                    nameof(scores),
                    $"Score must be between {CompetencyMatrix.NotEvaluatedScore} and {CompetencyMatrix.MaxScore}.");
        }

        foreach (var competencyScore in scores)
        {
            _matrixRows.Add(CompetencyMatrix.Create(
                Id,
                competencyScore.CompetencyId,
                competencyScore.Score,
                competencyScore.Comment));
        }

        SummaryComment = summaryComment;
        Status = InterviewStatus.WaitingForVerdict;
    }

    /// <summary>
    /// Отменяет запланированное интервью.
    /// </summary>
    public void Cancel()
    {
        EnsureStatus(InterviewStatus.Scheduled);
        Status = InterviewStatus.Canceled;
    }

    /// <summary>
    /// Завершает интервью после вынесения решения согласующим.
    /// </summary>
    public void Complete()
    {
        EnsureStatus(InterviewStatus.WaitingForVerdict);
        Status = InterviewStatus.Completed;
    }

    /// <summary>
    /// Фиксирует вердикт решалы и завершает интервью.
    /// </summary>
    public Verdict RecordVerdict(int userId, DeciderVerdict decision, string? comment)
    {
        EnsureStatus(InterviewStatus.WaitingForVerdict);

        if (Verdict is not null)
            throw new DomainException("Verdict has already been recorded.");

        var verdict = Verdict.Record(Id, userId, decision, comment);
        Complete();
        return verdict;
    }

    private static Interview Schedule(
        int candidateId,
        int vacancyId,
        Guid processId,
        int hrId,
        DateTime date)
    {
        if (candidateId <= 0)
            throw new ArgumentOutOfRangeException(nameof(candidateId));
        if (vacancyId <= 0)
            throw new ArgumentOutOfRangeException(nameof(vacancyId));
        if (hrId <= 0)
            throw new ArgumentOutOfRangeException(nameof(hrId));
        if (date <= DateTime.UtcNow)
            throw new ArgumentException("Date must be in the future.");

        return new Interview
        {
            CandidateId = candidateId,
            VacancyId = vacancyId,
            ProcessId = processId,
            HrId = hrId,
            Date = date,
            Status = InterviewStatus.Scheduled
        };
    }

    private void EnsureAssignedHr(int hrId)
    {
        if (HrId != hrId)
            throw new DomainException("Only the assigned HR can modify this interview protocol.");
    }

    private void EnsureStatus(InterviewStatus expected)
    {
        if (Status != expected)
            throw new InvalidStatusTransitionException(
                $"Interview status must be {expected}, but was {Status}.");
    }
}