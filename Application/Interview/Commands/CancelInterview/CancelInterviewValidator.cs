using FluentValidation;

namespace Application.Interview.Commands.CancelInterview;

public class CancelInterviewValidator : AbstractValidator<CancelInterviewCommand>
{
    public CancelInterviewValidator()
    {
        RuleFor(x => x.InterviewId)
            .GreaterThan(0);
    }
}