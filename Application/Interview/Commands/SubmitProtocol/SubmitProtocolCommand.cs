using Application.Interview.DTO;
using MediatR;

namespace Application.Interview.Commands.SubmitProtocol;

public record SubmitProtocolCommand(
    int InterviewId,
    int HrId,
    string? SummaryComment,
    IReadOnlyList<CompetencyScoreInput> Scores) : IRequest<int>;