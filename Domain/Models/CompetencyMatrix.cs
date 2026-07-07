using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models;

// [TODO] - Обсудить свойство "Кандидат"
public class CompetencyMatrix
{
    // { InterviewId, CompetencyId} составной ключ
    public int InterviewId { get; set; }
    public int CompetencyId { get; set; }

    public int Score { get; set; }
    public string? Comment { get; set; }

    // навигационные свойства
    public Interview Interview { get; set; }
    public Competency Competency { get; set; }
}
