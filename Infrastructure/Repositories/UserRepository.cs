using Application.Abstractions.Repositories;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository(BarsContext context) : IUserRepository
{
    public Task<bool> ExistsByIdAsync(int id, CancellationToken cancellationToken)
    {
        return context.Users.AnyAsync(user => user.Id == id, cancellationToken);
    }

    public Task<bool> IsAdminAsync(int id, CancellationToken cancellationToken)
    {
        return context.Users.AnyAsync(
            user => user.Id == id && user.Role == UserRole.Admin,
            cancellationToken);
    }
}