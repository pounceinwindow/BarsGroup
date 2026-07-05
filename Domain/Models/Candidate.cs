using System.ComponentModel.DataAnnotations;

namespace Domain.Models;

public class Candidate
{
    public int Id { get; set; }
    public string FullName { get; set; }
    [Phone] public string Phone { get; set; }
    public string City { get; set; }
    public string[] Education { get; set; }
    public string[] PreviousWork { get; set; }
    public string Status { get; set; }
    
    // [TODO: Артур] - добавить навыки кандидата (массив или таблица?)
    // public string[] Skills { get; set; }
}
