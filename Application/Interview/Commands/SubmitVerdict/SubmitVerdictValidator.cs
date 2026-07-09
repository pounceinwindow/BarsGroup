using Domain.Enums;
using FluentValidation;

namespace Application.Interview.Commands.SubmitVerdict;

public class SubmitVerdictValidator : AbstractValidator<SubmitVerdictCommand>
{
    public SubmitVerdictValidator()
    {
        RuleFor(x => x.InterviewId)
            .GreaterThan(0);

        RuleFor(x => x.DeciderId)
            .GreaterThan(0);

        RuleFor(x => x.Decision)
            .IsInEnum();
    }
}