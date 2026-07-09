using Domain.Enums;
using MediatR;

namespace Application.Interview.Commands.SubmitVerdict;

public record SubmitVerdictCommand(
    int InterviewId,
    int DeciderId,
    DeciderVerdict Decision,
    string? Comment) : IRequest<int>;