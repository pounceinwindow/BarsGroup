using Application.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CompetencyRepository(BarsContext context) : ICompetencyRepository
{
    public async Task<bool> AllCompetenciesExists(
        IReadOnlyCollection<int> competencyIds,
        CancellationToken cancellationToken)
    {
        if (competencyIds.Count == 0)
            return false;

        var distinctIds = competencyIds.Distinct().ToList();
        var existingCount = await context.Competencies
            .CountAsync(competency => distinctIds.Contains(competency.Id), cancellationToken);

        return existingCount == distinctIds.Count;
    }

    public async Task AddAsync(Domain.Models.Competency competence, CancellationToken cancellationToken)
    {
        await context.Competencies.AddAsync(competence, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Domain.Models.Competency?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await context.Competencies.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task UpdateAsync(Domain.Models.Competency competence, CancellationToken cancellationToken)
    {
        context.Competencies.Update(competence);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteByIdAsync(int id, CancellationToken cancellationToken)
    {
        var competence = await context.Competencies.FindAsync(new object[] { id }, cancellationToken);
        if (competence != null)
        {
            context.Competencies.Remove(competence);
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(string name, CancellationToken cancellationToken)
    {
        return await context.Competencies.AnyAsync(c => c.Name == name, cancellationToken);
    }
}