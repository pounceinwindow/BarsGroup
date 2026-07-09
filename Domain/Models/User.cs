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
    public int? AssignedBy { get; private set; }
    public DateOnly? RevokedAt { get; private set; }
    public int? RevokedBy { get; private set; }

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
        DateOnly createdAt,
        int? assignedBy = null)
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
            CreatedAt = createdAt,
            AssignedBy = assignedBy
        };
    }

    /// <summary>
    /// Отзывает доступ пользователя к системе.
    /// </summary>
    public void Revoke(DateOnly revokedAt, int revokedBy)
    {
        RevokedAt = revokedAt;
        RevokedBy = revokedBy;
    }

    /// <summary>
    /// Обновляет роль пользователя.
    /// </summary>
    public void UpdateRole(UserRole role, int? assignedBy = null)
    {
        Role = role;
        if (assignedBy != null)
        {
            AssignedBy = assignedBy;
        }
    }
}