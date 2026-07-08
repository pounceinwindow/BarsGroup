using FluentValidation;

namespace Application.Interview.Commands.CancelInterview;

public class CancelInterviewValidator : AbstractValidator<CancelInterviewCommand>
{
    public CancelInterviewValidator()
    {
        RuleFor(x => x.InterviewId)
            .GreaterThan(0);

        RuleFor(x => x.HrId)
            .GreaterThan(0);
    }
}