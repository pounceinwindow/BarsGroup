using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Domain.Models;

public class Candidate
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string? Telegram { get; set; }
    public string City { get; set; }
    public string[] Education { get; set; }
    public string[] PreviousWork { get; set; }
    public CandidateStatus Status { get; set; }
    public string[] Skills { get; set; } // [TODO: Артур] - оставить массив или вынести в отдельную таблицу?
}
