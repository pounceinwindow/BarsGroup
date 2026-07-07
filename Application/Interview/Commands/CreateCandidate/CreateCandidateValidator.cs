using FluentValidation;

namespace Application.Interview.Commands.CreateCandidate;

public class CreateCandidateValidator : AbstractValidator<CreateCandidateCommand>
{
    public CreateCandidateValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(50);
        RuleFor(x => x.Phone)
            .NotEmpty()
            .Matches(@"^\+?[1-9]\d{1,14}$")
            .WithMessage("Phone number is invalid.");
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Email is invalid.");
        RuleFor(x => x.Telegram)
            .Matches(@"^@[A-Za-z0-9_]{5,32}$")
            .WithMessage("Telegram handle is invalid.")
            .When(x => x.Telegram is not null);
        RuleFor(x => x.City)
            .NotEmpty()
            .MaximumLength(100);
        RuleFor(x => x.Education)
            .Must(a => a.Length <= 20)
            .WithMessage("Must be less than 20 elements.");
        RuleForEach(x => x.Education)
            .NotEmpty()
            .MaximumLength(200); // правила для каждого элемента массива
        RuleFor(x => x.PreviousWork)
            .NotNull()
            .Must(a => a.Length <= 20)
            .WithMessage("Must be less than 20 elements.");
        RuleForEach(x => x.PreviousWork)
            .NotNull()
            .NotEmpty()
            .MaximumLength(200); // правила для каждого элемента массива
        RuleFor(x => x.Skills)
            .NotNull()
            .Must(a => a.Length <= 20)
            .WithMessage("Must be less than 20 elements.");
        RuleForEach(x => x.Skills)
            .NotEmpty()
            .MaximumLength(200); // правила для каждого элемента массива
    }
}