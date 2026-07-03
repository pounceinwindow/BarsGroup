using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models;

public class Interview
{
    public int Id { get; set; }
    public int VacancyId { get; set; }
    public int CandidateId { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; }

    // навигационные свойства
    public Vacancy Vacancy { get; set; }
    public Candidate Candidate { get; set; }
}
