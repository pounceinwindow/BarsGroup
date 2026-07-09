using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interview.DTO;

public class InterviewDto
{
    public int Id { get; set; }
    public string Candidate { get; set; }
    public string Vacancy { get; set; }
    public InterviewStatus Status { get; set; }
    public string Date { get; set; }
}
