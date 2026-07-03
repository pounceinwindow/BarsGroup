using System.ComponentModel.DataAnnotations;

namespace Domain.Models;

public class Candidate
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    [Phone] public string Phone { get; set; }
    public string City { get; set; }
    public string Education { get; set; }
    public string PreviousWork { get; set; }
    public string Status { get; set; }
}
