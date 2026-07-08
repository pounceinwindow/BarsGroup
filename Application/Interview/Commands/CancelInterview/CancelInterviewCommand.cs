using MediatR;

namespace Application.Interview.Commands.CancelInterview;

public record CancelInterviewCommand(int InterviewId, int HrId) : IRequest<int>;