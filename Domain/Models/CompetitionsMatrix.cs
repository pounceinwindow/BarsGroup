using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models;

public class CompetitionsMatrix
{
    // { CandidateId, CompetitionId} составной ключ
    public int CandidateId { get; set; }
    public int CompetitionId { get; set; }

    public int Score { get; set; }
    public string Comment { get; set; }

    // навигационные свойства
    public Candidate Candidate { get; set; }
    public Competition Competition { get; set; }
}
