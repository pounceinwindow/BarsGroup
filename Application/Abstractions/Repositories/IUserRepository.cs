namespace Application.Abstractions.Repositories;

public interface IUserRepository
{
    Task<bool> ExistsByIdAsync(int id, CancellationToken cancellationToken);

    Task<bool> IsAdminAsync(int id, CancellationToken cancellationToken);
}