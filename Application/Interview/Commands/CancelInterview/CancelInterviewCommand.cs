using MediatR;

namespace Application.Interview.Commands.CancelInterview;

public record CancelInterviewCommand(int InterviewId) : IRequest<int>;