using Domain.Enums;

namespace Domain.Models;

public class User
{
    public int Id { get; private set; }
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string Username { get; private set; } = null!;
    public string? PasswordHash { get; private set; }
    public UserRole Role { get; private set; }
    public DateOnly CreatedAt { get; private set; }
    public DateOnly? RevokedAt { get; private set; }
    public string? RevokedBy { get; private set; }

    private User()
    {
    }

    /// <summary>
    /// Создаёт нового пользователя системы.
    /// </summary>
    public static User Create(
        string username,
        string firstName,
        string lastName,
        string? passwordHash,
        UserRole role,
        DateOnly createdAt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName);

        return new User
        {
            Username = username,
            FirstName = firstName,
            LastName = lastName,
            PasswordHash = passwordHash,
            Role = role,
            CreatedAt = createdAt
        };
    }

    /// <summary>
    /// Отзывает доступ пользователя к системе.
    /// </summary>
    public void Revoke(DateOnly revokedAt, string revokedBy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(revokedBy);
        RevokedAt = revokedAt;
        RevokedBy = revokedBy;
    }
}