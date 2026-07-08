using Application.Abstractions.Repositories;
using Application.Interview.DTO;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CandidateRepository(BarsContext context) : ICandidateRepository
{
    public async Task AddAsync(Candidate candidate, CancellationToken cancellationToken)
    {
        await context.Candidates.AddAsync(candidate, cancellationToken);
    }

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return context.Candidates.AnyAsync(c => c.Email == email, cancellationToken);
    }

    public Task<bool> ExistsByIdAsync(int id, CancellationToken cancellationToken)
    {
        return context.Candidates.AnyAsync(c => c.Id == id, cancellationToken);
    }

    public Task<CandidateResponse?> GetByIdDetailedAsync(int id, CancellationToken cancellationToken)
    {
        return context.Candidates
            .AsNoTracking()
            .Where(candidate => candidate.Id == id)
            .Select(candidate => new CandidateResponse(
                candidate.Id,
                candidate.FullName,
                candidate.Phone,
                candidate.Email,
                candidate.Telegram,
                candidate.City,
                candidate.Education,
                candidate.PreviousWork,
                candidate.Skills,
                candidate.Status,
                candidate.Interviews
                    .GroupBy(interview => interview.ProcessId)
                    // Сортируем по датам из группы
                    .OrderByDescending(group => group.Max(interview => interview.Date))
                    .Select(group => new ApplicationProcessResponse(
                        group.Key,
                        // Все этапы одного ProcessId относятся к одной вакансии, берём любое из группы.
                        group.First().VacancyId,
                        group.First().Vacancy.Name,
                        group
                            .OrderBy(interview => interview.Date)
                            .Select(interview => new InterviewResponse(
                                interview.Id,
                                interview.Date,
                                interview.Status,
                                interview.SummaryComment,
                                interview.Verdict != null ? interview.Verdict.Decision : null,
                                interview.Verdict != null ? interview.Verdict.Comment : null))
                            .ToList()))
                    .ToList()))
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
    }
}