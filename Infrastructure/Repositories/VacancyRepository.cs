using Application.Abstractions.Repositories;
using Application.Exceptions;
using Application.Vacancy;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class VacancyRepository(BarsContext context) : IVacancyRepository
{
    public async Task AddAsync(Vacancy vacancy, CancellationToken cancellationToken)
    {
        await context.Vacancies.AddAsync(vacancy, cancellationToken);
    }

    public async Task DeleteByIdAsync(int id, CancellationToken cancellationToken)
    {
        var vacancy = await context.Vacancies.FirstOrDefaultAsync(item => item.Id == id, cancellationToken)
                      ?? throw new NotFoundException($"Vacancy with id {id} was not found.");

        context.Vacancies.Remove(vacancy);
    }

    public async Task DeleteByNameAsync(string vacancyName, CancellationToken cancellationToken)
    {
        var vacancy = await context.Vacancies.FirstOrDefaultAsync(item => item.Name == vacancyName, cancellationToken)
                      ?? throw new NotFoundException($"Vacancy with name '{vacancyName}' was not found.");

        context.Vacancies.Remove(vacancy);
    }

    public async Task AddCompetencyByIdAsync(int vacancyId, int competencyId, CancellationToken cancellationToken)
    {
        if (!await ExistsByIdAsync(vacancyId, cancellationToken))
            throw new NotFoundException($"Vacancy with id {vacancyId} was not found.");

        if (!await context.Competencies.AnyAsync(competency => competency.Id == competencyId, cancellationToken))
            throw new NotFoundException($"Competency with id {competencyId} was not found.");

        var alreadyLinked = await context.VacancyCompetencies.AnyAsync(
            link => link.VacancyId == vacancyId && link.CompetencyId == competencyId,
            cancellationToken);

        if (alreadyLinked)
            return;

        await context.VacancyCompetencies.AddAsync(
            VacancyCompetency.Create(vacancyId, competencyId),
            cancellationToken);
    }

    public async Task AddCompetencyByNameAsync(
        string vacancyName,
        string competencyName,
        CancellationToken cancellationToken)
    {
        var vacancyId = await context.Vacancies
            .Where(vacancy => vacancy.Name == vacancyName)
            .Select(vacancy => vacancy.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (vacancyId == 0)
            throw new NotFoundException($"Vacancy with name '{vacancyName}' was not found.");

        var competencyId = await context.Competencies
            .Where(competency => competency.Name == competencyName)
            .Select(competency => competency.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (competencyId == 0)
            throw new NotFoundException($"Competency with name '{competencyName}' was not found.");

        await AddCompetencyByIdAsync(vacancyId, competencyId, cancellationToken);
    }

    public async Task RemoveCompetencyByIdAsync(
        int vacancyId,
        int competencyId,
        CancellationToken cancellationToken)
    {
        var link = await context.VacancyCompetencies.FirstOrDefaultAsync(
            item => item.VacancyId == vacancyId && item.CompetencyId == competencyId,
            cancellationToken);

        if (link is null)
            throw new NotFoundException(
                $"Competency {competencyId} is not linked to vacancy {vacancyId}.");

        context.VacancyCompetencies.Remove(link);
    }

    public async Task RemoveCompetencyByNameAsync(
        string vacancyName,
        string competencyName,
        CancellationToken cancellationToken)
    {
        var vacancyId = await context.Vacancies
            .Where(vacancy => vacancy.Name == vacancyName)
            .Select(vacancy => vacancy.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (vacancyId == 0)
            throw new NotFoundException($"Vacancy with name '{vacancyName}' was not found.");

        var competencyId = await context.Competencies
            .Where(competency => competency.Name == competencyName)
            .Select(competency => competency.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (competencyId == 0)
            throw new NotFoundException($"Competency with name '{competencyName}' was not found.");

        await RemoveCompetencyByIdAsync(vacancyId, competencyId, cancellationToken);
    }

    public Task<bool> ExistsByIdAsync(int id, CancellationToken cancellationToken)
    {
        return context.Vacancies.AnyAsync(vacancy => vacancy.Id == id, cancellationToken);
    }

    public Task<bool> ExistsByNameAsync(string vacancyName, CancellationToken cancellationToken)
    {
        return context.Vacancies.AnyAsync(vacancy => vacancy.Name == vacancyName, cancellationToken);
    }

    public async Task<IReadOnlyList<int>> GetCompetencyIdsAsync(
        int vacancyId,
        CancellationToken cancellationToken)
    {
        return await context.VacancyCompetencies
            .Where(vacancyCompetency => vacancyCompetency.VacancyId == vacancyId)
            .Select(vacancyCompetency => vacancyCompetency.CompetencyId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<VacancyDto>> GetAll()
    {
        return await context.Vacancies
            .AsNoTracking()
            .Select(x => new VacancyDto(x.Name))
            .ToListAsync();
    }
}