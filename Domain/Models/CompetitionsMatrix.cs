using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models;

public class CompetitionsMatrix
{
    // { InterviewId, CompetitionId} составной ключ
    public int InterviewId { get; set; }
    public int CompetitionId { get; set; }

    public int Score { get; set; }
    public string Comment { get; set; }

    // навигационные свойства
    public Interview Interview { get; set; }
    public Competition Competition { get; set; }
}
