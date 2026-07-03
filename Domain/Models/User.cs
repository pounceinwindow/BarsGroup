using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PasswordHash { get; set; }
    public string Role { get; set; }
    public DateOnly CreatedAt { get; set; }
    public DateOnly RevokedAt { get; set; }
    public string RevokedBy { get; set; }
}
