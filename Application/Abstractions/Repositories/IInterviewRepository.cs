using Domain.Models;

namespace Application.Abstractions.Repositories;

public interface IInterviewRepository
{
    Task AddAsync(Domain.Models.Interview interview, CancellationToken cancellationToken);

    /// <summary>
    /// Загружает интервью с матрицей для доменных операций (SubmitProtocol и т.п.).
    /// </summary>
    Task<Domain.Models.Interview?> GetByIdAsync(int interviewId, CancellationToken cancellationToken);

    /// <summary>
    /// Есть ли у кандидата на вакансию интервью в активном статусе (Scheduled / WaitingForVerdict).
    /// </summary>
    Task<bool> HasActiveAsync(int candidateId, int vacancyId, CancellationToken cancellationToken);

    /// <summary>
    /// Проверяет, что ProcessId уже существует для пары кандидат+вакансия (следующий этап того же отклика).
    /// </summary>
    Task<bool> ProcessExistsAsync(
        Guid processId,
        int candidateId,
        int vacancyId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Проверяет, что интервью существует.
    /// </summary>
    Task<bool> InterviewExistsAsync(int interviewId, CancellationToken cancellationToken);

    Task AddVerdictAsync(Verdict verdict, CancellationToken cancellationToken);
}