using FluentValidation;

namespace Application.Interview.Commands.ScheduleInterview;

public class ScheduleInterviewValidator : AbstractValidator<ScheduleInterviewCommand>
{
    public ScheduleInterviewValidator()
    {
        RuleFor(x => x.CandidateId)
            .GreaterThan(0);
        RuleFor(x => x.VacancyId)
            .GreaterThan(0);
        RuleFor(x => x.HrId)
            .GreaterThan(0);
        RuleFor(x => x.Date)
            .Must(date => date > DateTime.Now)
            .WithMessage("Date must be in the future.");
        // ProcessId = null — новый отклик; непустой Guid — следующий этап того же отклика.
        RuleFor(x => x.ProcessId)
            .Must(processId => processId is null || processId != Guid.Empty)
            .WithMessage("ProcessId cannot be empty.");
    }
}