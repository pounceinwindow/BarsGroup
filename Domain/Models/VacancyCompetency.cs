namespace Domain.Models;

/// <summary>
/// Связь компетенции с вакансией.
/// </summary>
public class VacancyCompetency
{
    public int VacancyId { get; private set; }
    public int CompetencyId { get; private set; }

    public Vacancy Vacancy { get; private set; } = null!;
    public Competency Competency { get; private set; } = null!;

    private VacancyCompetency()
    {
    }

    /// <summary>
    /// Создаёт привязку компетенции к вакансии.
    /// </summary>
    public static VacancyCompetency Create(int vacancyId, int competencyId)
    {
        if (vacancyId <= 0)
            throw new ArgumentOutOfRangeException(nameof(vacancyId));
        if (competencyId <= 0)
            throw new ArgumentOutOfRangeException(nameof(competencyId));

        return new VacancyCompetency
        {
            VacancyId = vacancyId,
            CompetencyId = competencyId
        };
    }
}