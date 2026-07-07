using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Models;

public class Candidate
{
    public int Id { get; private set; }
    public string FullName { get; private set; } = null!;
    public string Phone { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string? Telegram { get; private set; }
    public string City { get; private set; } = null!;
    public string[]? Education { get; private set; }
    public string[]? PreviousWork { get; private set; }
    public CandidateStatus Status { get; private set; }
    public string[] Skills { get; private set; } = [];

    // Интервью создаются отдельно (Interview.Schedule*), кандидат их не добавляет —
    // поэтому read-only навигация без приватного списка в домене.
    public IReadOnlyCollection<Interview> Interviews { get; private set; } = null!;

    private Candidate()
    {
    }

    /// <summary>
    /// Создаёт нового кандидата в статусе поиска работы.
    /// </summary>
    public static Candidate Create(
        string fullName,
        string phone,
        string email,
        string? telegram,
        string city,
        string[]? education,
        string[]? previousWork,
        string[] skills)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fullName);
        ArgumentException.ThrowIfNullOrWhiteSpace(phone);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(city);
        ArgumentNullException.ThrowIfNull(skills);

        return new Candidate
        {
            FullName = fullName,
            Phone = phone,
            Email = email,
            Telegram = telegram,
            City = city,
            Education = education,
            PreviousWork = previousWork,
            Skills = skills,
            Status = CandidateStatus.LookingForWork
        };
    }

    /// <summary>
    /// Переводит кандидата в статус «Принят на работу».
    /// </summary>
    public void Hire()
    {
        if (Status == CandidateStatus.Archived)
            throw new InvalidStatusTransitionException("Archived candidate cannot be hired.");

        Status = CandidateStatus.Hired;
    }

    /// <summary>
    /// Переводит кандидата в архив.
    /// </summary>
    public void Archive()
    {
        Status = CandidateStatus.Archived;
    }
}