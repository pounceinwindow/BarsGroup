using Domain.Models;

namespace Application.Abstractions.Repositories
{
    public interface IVacancyRepository
    {
        Task AddAsync(Vacancy vacancy);

        //Аккуратно, зависимости с Interview
        Task DeleteByIdAsync(int id);

        //Аккуратно, зависимости с Interview
        Task DeleteByNameAsync(string vacancyName);

        Task AddCompetencyByIdAsync(int vacancyId, int competencyId);

        Task AddCompetencyByNameAsync(string vacancyName, string competencyName);

        Task RemoveCompetencyByIdAsync(int vacancyId, int competencyId);

        Task RemoveCompetencyByNameAsync(string vacancyName, string competencyName);

        Task ExistsByNameAsync(string vacancyName);
    }
}
