namespace Application.Abstractions.Repositories;
using Domain.Models;

public interface ICompetencyRepository
{
    /// <summary>
    /// Проверяет, что все компетенции из списка существуют.
    /// </summary>
    Task<bool> AllCompetenciesExists(
        IReadOnlyCollection<int> competencyIds,
        CancellationToken cancellationToken);

    Task AddAsync(Competency competence, CancellationToken cancellationToken);

    Task<Competency?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task UpdateAsync(Competency competence, CancellationToken cancellationToken);

    Task DeleteByIdAsync(int id, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(string name, CancellationToken cancellationToken);
}