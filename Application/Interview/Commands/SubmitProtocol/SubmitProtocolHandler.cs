using Application.Abstractions;
using Application.Abstractions.Repositories;
using Application.Exceptions;
using Domain.Models;
using MediatR;

namespace Application.Interview.Commands.SubmitProtocol;

public class SubmitProtocolHandler : IRequestHandler<SubmitProtocolCommand, int>
{
    private readonly IInterviewRepository _interviews;
    private readonly IVacancyRepository _vacancies;
    private readonly ICompetencyRepository _competencies;
    private readonly IUnitOfWork _unitOfWork;

    public SubmitProtocolHandler(
        IInterviewRepository interviews,
        IVacancyRepository vacancies,
        ICompetencyRepository competencies,
        IUnitOfWork unitOfWork)
    {
        _interviews = interviews;
        _vacancies = vacancies;
        _competencies = competencies;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(SubmitProtocolCommand request, CancellationToken cancellationToken)
    {
        var interview = await _interviews.GetByIdAsync(request.InterviewId, cancellationToken)
                        ?? throw new NotFoundException(
                            $"The interview with id {request.InterviewId} was not found.");

        var scoreCompetencyIds = request.Scores
            .Select(score => score.CompetencyId)
            .ToList();

        if (!await _competencies.AllCompetenciesExists(scoreCompetencyIds, cancellationToken))
        {
            throw new NotFoundException("Incorrect competency list. Some competencies were not found.");
        }

        // Протокол должен покрывать ровно шаблон компетенций вакансии
        var vacancyCompetencyIds = await _vacancies.GetCompetencyIdsAsync(
            interview.VacancyId,
            cancellationToken);

        if (vacancyCompetencyIds.Count == 0)
        {
            throw new ConflictException(
                $"Vacancy {interview.VacancyId} has no competencies configured.");
        }

        if (!SameCompetencySet(scoreCompetencyIds, vacancyCompetencyIds))
        {
            throw new ConflictException(
                "Scores must match all vacancy competencies.");
        }

        var protocolScores = request.Scores
            .Select(score => new ProtocolCompetencyScore(score.CompetencyId, score.Score, score.Comment))
            .ToList();

        interview.SubmitProtocol(request.SummaryComment, protocolScores);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return interview.Id;
    }

    private static bool SameCompetencySet(
        List<int> submitted,
        IReadOnlyCollection<int> vacancyTemplate)
    {
        if (submitted.Count != vacancyTemplate.Count)
            return false;

        // Дубликаты в сабмите не считаем совпадением с шаблоном.
        var submittedSet = submitted.ToHashSet();
        return submittedSet.Count == submitted.Count && submittedSet.SetEquals(vacancyTemplate);
    }
}