namespace Application.Abstractions.Repositories;

public interface IInterviewRepository
{
    Task AddAsync(Domain.Models.Interview interview, CancellationToken cancellationToken);

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
}