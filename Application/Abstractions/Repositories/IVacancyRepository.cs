namespace Application.Abstractions.Repositories;

public interface IVacancyRepository
{
    Task<bool> ExistsByIdAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Id компетенций из шаблона вакансии (VacancyCompetency).
    /// </summary>
    Task<IReadOnlyList<int>> GetCompetencyIdsAsync(int vacancyId, CancellationToken cancellationToken);
}