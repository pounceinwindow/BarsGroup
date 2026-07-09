using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Candidate.DTO;

public class CandidateFilters
{
    public string Search { get; set; } = string.Empty;
    public CandidateStatus Status { get; set; } = CandidateStatus.All;

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
