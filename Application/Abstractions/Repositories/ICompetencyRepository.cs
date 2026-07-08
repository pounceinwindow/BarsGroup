namespace Application.Abstractions.Repositories;

public interface ICompetencyRepository
{
    /// <summary>
    /// Проверяет, что все компетенции из списка существуют.
    /// </summary>
    Task<bool> AllCompetenciesExists(
        IReadOnlyCollection<int> competencyIds,
        CancellationToken cancellationToken);
}