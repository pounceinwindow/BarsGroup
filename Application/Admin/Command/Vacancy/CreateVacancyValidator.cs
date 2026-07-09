using FluentValidation;

namespace Application.Admin.Command.Vacancy;

public class CreateVacancyValidator : AbstractValidator<CreateVacancyCommand>
{
    public CreateVacancyValidator()
    {
        RuleFor(x => x.VacancyName)
            .NotEmpty().WithMessage("Название вакансии не может быть пустым")
            .MaximumLength(100).WithMessage("Название вакансии не может превышать 100 символов");

        RuleFor(x => x.CompetencyIds)
            .NotNull().WithMessage("Список компетенций обязателен")
            .Must(x => x != null && x.Count >= 3 && x.Count <= 15)
            .WithMessage("Выберите от 3 до 15 компетенций");
    }
}
