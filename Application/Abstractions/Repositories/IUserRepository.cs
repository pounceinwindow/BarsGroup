using Domain.Models;

namespace Application.Abstractions.Repositories
{
    public interface IUserRepository
    {
        Task AddAsync(User user);

        Task<User> GetUserAsync(int id);

        Task MarkRevokedAsync(int id);

        Task<bool> ExistsByIdAsync(int id);
    }
}
