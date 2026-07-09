using Domain.Models;

namespace Application.Abstractions.Repositories;

public interface IVacancyRepository
{
    Task AddAsync(Vacancy vacancy, CancellationToken cancellationToken);
    
    Task<Vacancy?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task UpdateAsync(Vacancy vacancy, CancellationToken cancellationToken);

    //Аккуратно, зависимости с Interview
    Task DeleteByIdAsync(int id, CancellationToken cancellationToken);

    //Аккуратно, зависимости с Interview
    Task DeleteByNameAsync(string vacancyName, CancellationToken cancellationToken);

    Task AddCompetencyByIdAsync(int vacancyId, int competencyId, CancellationToken cancellationToken);

    Task AddCompetencyByNameAsync(string vacancyName, string competencyName, CancellationToken cancellationToken);

    Task RemoveCompetencyByIdAsync(int vacancyId, int competencyId, CancellationToken cancellationToken);

    Task RemoveCompetencyByNameAsync(string vacancyName, string competencyName,  CancellationToken cancellationToken);

    Task<bool> ExistsByIdAsync(int id, CancellationToken cancellationToken);

    Task<bool> ExistsByNameAsync(string vacancyName, CancellationToken cancellationToken);

    /// <summary>
    /// Id компетенций из шаблона вакансии (VacancyCompetency).
    /// </summary>
    Task<IReadOnlyList<int>> GetCompetencyIdsAsync(int vacancyId, CancellationToken cancellationToken);
}