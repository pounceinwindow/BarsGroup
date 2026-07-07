namespace Domain.Models;

// [TODO] - Обсудить свойство "Кандидат"
public class CompetencyMatrix
{
    public const int NotEvaluatedScore = 0;
    public const int MinScore = 1;
    public const int MaxScore = 5;

    public int InterviewId { get; private set; }
    public int CompetencyId { get; private set; }
    public int Score { get; private set; }
    public string? Comment { get; private set; }

    public Interview Interview { get; private set; } = null!;
    public Competency Competency { get; private set; } = null!;

    private CompetencyMatrix()
    {
    }

    /// <summary>
    /// Создаёт неизменяемую строку матрицы компетенций с оценкой и комментарием.
    /// </summary>
    public static CompetencyMatrix Create(int interviewId, int competencyId, int score, string? comment)
    {
        if (competencyId <= 0)
            throw new ArgumentOutOfRangeException(nameof(competencyId));

        if (score is < NotEvaluatedScore or > MaxScore)
            throw new ArgumentOutOfRangeException(
                nameof(score),
                $"Score must be between {NotEvaluatedScore} and {MaxScore}.");

        return new CompetencyMatrix
        {
            InterviewId = interviewId,
            CompetencyId = competencyId,
            Score = score,
            Comment = comment
        };
    }

    /// <summary>
    /// Возвращает true, если компетенция оценена по шкале 1–5.
    /// </summary>
    public bool IsEvaluated => Score is >= MinScore and <= MaxScore;
}