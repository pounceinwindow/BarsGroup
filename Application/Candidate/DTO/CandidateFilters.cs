using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Candidate.DTO;

public class CandidateFilters
{
    public string Search { get; set; } = string.Empty;
    public CandidateStatus Status { get; set; } = CandidateStatus.All;
}
