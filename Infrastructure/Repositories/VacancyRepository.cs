using Application.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class VacancyRepository(BarsContext context) : IVacancyRepository
{
    public Task<bool> ExistsByIdAsync(int id, CancellationToken cancellationToken)
    {
        return context.Vacancies.AnyAsync(vacancy => vacancy.Id == id, cancellationToken);
    }
}