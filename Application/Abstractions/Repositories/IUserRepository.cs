using Domain.Models;

namespace Application.Abstractions.Repositories;

public interface IUserRepository
{
    Task AddAsync(User user, CancellationToken cancellationToken);

    Task<User> GetUserAsync(int id, CancellationToken cancellationToken);

    Task MarkRevokedAsync(int id, int revokedBy, CancellationToken cancellationToken);

    Task<bool> ExistsByIdAsync(int id, CancellationToken cancellationToken);

    Task<bool> IsAdminAsync(int id, CancellationToken cancellationToken);

    Task<bool> IsDeciderAsync(int id, CancellationToken cancellationToken);

    Task<User?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken);
}