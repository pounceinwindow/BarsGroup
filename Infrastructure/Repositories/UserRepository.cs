using Application.Abstractions.Repositories;
using Application.Exceptions;
using Domain.Enums;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository(BarsContext context) : IUserRepository
{
    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        await context.Users.AddAsync(user, cancellationToken);
    }

    public async Task<User> GetUserAsync(int id, CancellationToken cancellationToken)
    {
        return await context.Users.FirstOrDefaultAsync(user => user.Id == id, cancellationToken)
               ?? throw new NotFoundException($"User with id {id} was not found.");
    }

    public async Task MarkRevokedAsync(int id, CancellationToken cancellationToken)
    {
        var user = await context.Users.FirstOrDefaultAsync(item => item.Id == id, cancellationToken)
                   ?? throw new NotFoundException($"User with id {id} was not found.");

        user.Revoke(DateOnly.FromDateTime(DateTime.UtcNow), "Admin");
    }

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

    public Task<bool> IsDeciderAsync(int id, CancellationToken cancellationToken)
    {
        return context.Users.AnyAsync(
            user => user.Id == id && user.Role == UserRole.Decider,
            cancellationToken);
    }
}