using Domain.Models;
using FluentValidation;

namespace Application.Interview.Commands.SubmitProtocol;

public class SubmitProtocolValidator : AbstractValidator<SubmitProtocolCommand>
{
    public SubmitProtocolValidator()
    {
        RuleFor(x => x.InterviewId)
            .GreaterThan(0);

        RuleFor(x => x.Scores)
            .NotNull()
            .Must(scores => scores.Count > 0)
            .WithMessage("Protocol must contain at least one competency score.");

        RuleForEach(x => x.Scores).ChildRules(score =>
        {
            score.RuleFor(x => x.CompetencyId)
                .GreaterThan(0);
            score.RuleFor(x => x.Score)
                .InclusiveBetween(CompetencyMatrix.NotEvaluatedScore, CompetencyMatrix.MaxScore);
        });
    }
}