using Application.Abstractions;
using Application.Abstractions.Repositories;
using Application.Exceptions;
using MediatR;

namespace Application.Interview.Commands.ScheduleInterview;

public class ScheduleInterviewHandler : IRequestHandler<ScheduleInterviewCommand, int>
{
    private readonly ICandidateRepository _candidates;
    private readonly IVacancyRepository _vacancies;
    private readonly IInterviewRepository _interviews;
    private readonly IUnitOfWork _unitOfWork;

    public ScheduleInterviewHandler(
        ICandidateRepository candidates,
        IVacancyRepository vacancies,
        IInterviewRepository interviews,
        IUnitOfWork unitOfWork)
    {
        _candidates = candidates;
        _vacancies = vacancies;
        _interviews = interviews;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(ScheduleInterviewCommand request, CancellationToken cancellationToken)
    {
        if (!await _candidates.ExistsByIdAsync(request.CandidateId, cancellationToken))
        {
            throw new NotFoundException($"Candidate with id {request.CandidateId} was not found.");
        }

        if (!await _vacancies.ExistsByIdAsync(request.VacancyId, cancellationToken))
        {
            throw new NotFoundException($"Vacancy with id {request.VacancyId} was not found.");
        }

        // На одну пару кандидат+вакансия одновременно только одно активное интервью.
        if (await _interviews.HasActiveAsync(request.CandidateId, request.VacancyId, cancellationToken))
        {
            throw new ConflictException(
                "An active interview already exists for this candidate and vacancy.");
        }

        Domain.Models.Interview interview;
        if (request.ProcessId is null)
        {
            interview = Domain.Models.Interview.ScheduleNewProcess(
                request.CandidateId,
                request.VacancyId,
                request.Date);
        }
        else
        {
            // Следующий этап должен ссылаться на уже существующий отклик (ProcessId) этой пары.
            if (!await _interviews.ProcessExistsAsync(
                    request.ProcessId.Value,
                    request.CandidateId,
                    request.VacancyId,
                    cancellationToken))
            {
                throw new NotFoundException(
                    $"Interview process {request.ProcessId.Value} was not found for this candidate and vacancy.");
            }

            interview = Domain.Models.Interview.ScheduleNextStage(
                request.CandidateId,
                request.VacancyId,
                request.ProcessId.Value,
                request.Date);
        }

        await _interviews.AddAsync(interview, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return interview.Id;
    }
}