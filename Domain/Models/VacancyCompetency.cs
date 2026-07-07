using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models;

// Смежная таблица связывающая Компетенции к конкретной вакансии
public class VacancyCompetency
{
    public int VacancyId { get; set; }
    public int CompetencyId { get; set; }

    // навигационные свойства
    public Vacancy Vacancy { get; set; }
    public Competency Competency { get; set; }
}
