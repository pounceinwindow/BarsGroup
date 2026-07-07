using Domain.Models;

namespace Application.Abstractions.Repositories
{
    public interface ICompetentionRepository
    {
        Task AddAsync(Competency competence);

        //Тут нужно аккуратно, есть зависимости с Vacancy и CompetencyMatrix
        Task DeleteByIdAsync(int id);

        Task<bool> ExistsAsync(string competentionName);
    }
}
