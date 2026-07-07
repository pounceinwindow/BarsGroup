using System;
using System.Collections.Generic;
using System.Text;
using Domain.Enums;

namespace Domain.Models;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PasswordHash { get; set; }
    public UserRole Role { get; set; }
    public DateOnly CreatedAt { get; set; }
    public DateOnly RevokedAt { get; set; }
    public string RevokedBy { get; set; }
}
