using Application.Abstractions;

namespace Infrastructure.Repositories;

public class UnitOfWork(BarsContext context) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return context.SaveChangesAsync(cancellationToken);
    }
}