using MediatR;

namespace Application.Interview.Commands.CreateCandidate;

public record CreateCandidateCommand(
    string FullName,
    string Phone,
    string Email,
    string? Telegram,
    string City,
    string[] Education,
    string[] PreviousWork,
    string[] Skills) : IRequest<Unit>;