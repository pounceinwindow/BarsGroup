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
}