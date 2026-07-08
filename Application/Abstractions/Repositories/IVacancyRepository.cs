namespace Application.Abstractions.Repositories;

public interface IVacancyRepository
{
    Task<bool> ExistsByIdAsync(int id, CancellationToken cancellationToken);
}