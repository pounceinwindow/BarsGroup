namespace Domain.Models;

/// <summary>
/// Оценка и комментарий HR по одной компетенции в протоколе собеседования.
/// </summary>
public sealed record ProtocolCompetencyScore(int CompetencyId, int Score, string? Comment);