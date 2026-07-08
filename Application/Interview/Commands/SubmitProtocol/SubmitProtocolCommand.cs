using Application.Interview.DTO;
using MediatR;

namespace Application.Interview.Commands.SubmitProtocol;

public record SubmitProtocolCommand(
    int InterviewId,
    string? SummaryComment,
    IReadOnlyList<CompetencyScoreInput> Scores) : IRequest<int>
{
}