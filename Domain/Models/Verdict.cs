using Domain.Enums;

namespace Domain.Models;

public class Verdict
{
    public int InterviewId { get; private set; }
    public int UserId { get; private set; }
    public DeciderVerdict Decision { get; private set; }
    public string? Comment { get; private set; }

    public Interview Interview { get; private set; } = null!;
    public User User { get; private set; } = null!;

    private Verdict()
    {
    }

    /// <summary>
    /// Фиксирует решение согласующего по интервью.
    /// </summary>
    public static Verdict Record(
        int interviewId,
        int userId,
        DeciderVerdict decision,
        string? comment)
    {
        if (interviewId <= 0)
            throw new ArgumentOutOfRangeException(nameof(interviewId));
        if (userId <= 0)
            throw new ArgumentOutOfRangeException(nameof(userId));

        return new Verdict
        {
            InterviewId = interviewId,
            UserId = userId,
            Decision = decision,
            Comment = comment
        };
    }
}