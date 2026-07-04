using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models;

// Смежная таблица связывающая Компетенции к конкретной вакансии
public class VacancyCompetition
{
    public int VacancyId { get; set; }
    public int CompetitionId { get; set; }

    // навигационные свойства
    public Vacancy Vacancy { get; set; }
    public Competition Competition { get; set; }
}
