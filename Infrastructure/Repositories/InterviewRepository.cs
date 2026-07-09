using Application.Abstractions.Repositories;
using Application.Interview.DTO;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class InterviewRepository(BarsContext context) : IInterviewRepository
{
    public async Task AddAsync(Domain.Models.Interview interview, CancellationToken cancellationToken)
    {
        await context.Interviews.AddAsync(interview, cancellationToken);
    }

    public Task<Domain.Models.Interview?> GetByIdAsync(int interviewId, CancellationToken cancellationToken)
    {
        return context.Interviews
            .Include(interview => interview.MatrixRows)
            .FirstOrDefaultAsync(interview => interview.Id == interviewId, cancellationToken);
    }

    public Task<bool> HasActiveAsync(int candidateId, int vacancyId, CancellationToken cancellationToken)
    {
        // Активные статусы: ещё не завершено и не отменено — параллельно второе назначать нельзя.
        return context.Interviews.AnyAsync(
            interview => interview.CandidateId == candidateId
                         && interview.VacancyId == vacancyId
                         && (interview.Status == InterviewStatus.Scheduled
                             || interview.Status == InterviewStatus.WaitingForVerdict),
            cancellationToken);
    }

    public Task<bool> ProcessExistsAsync(
        Guid processId,
        int candidateId,
        int vacancyId,
        CancellationToken cancellationToken)
    {
        return context.Interviews.AnyAsync(
            interview => interview.ProcessId == processId
                         && interview.CandidateId == candidateId
                         && interview.VacancyId == vacancyId,
            cancellationToken);
    }

    public Task<bool> InterviewExistsAsync(int interviewId, CancellationToken cancellationToken)
    {
        return context.Interviews.AnyAsync(interview => interview.Id == interviewId, cancellationToken);
    }

    public async Task<(List<InterviewDto>, int)> GetFilteredInterviews(
        InterviewFilters filters, 
        CancellationToken cancellationToken)
    {
        var interviews = context.Interviews
            .Include(x => x.Vacancy)
            .Include(x => x.Candidate)
            .AsNoTracking();

        if (filters.Status != InterviewStatus.All)
        {
            interviews = interviews
                .Where(x => x.Status == filters.Status);
        }

        if (!string.IsNullOrEmpty(filters.Search))
        {
            interviews = interviews
                .Where(x => x.Candidate.FullName.Contains(filters.Search));
        }

        if (!string.IsNullOrEmpty(filters.Vacancy))
        {
            interviews = interviews
                .Where(x => x.Vacancy.Name == filters.Vacancy);
        }

        var count = await interviews.CountAsync(cancellationToken);

        var items = await interviews
            .Skip((filters.Page - 1) * filters.PageSize)
            .Take(filters.PageSize)
            .Select(x => new InterviewDto
            {
                Id = x.Id,
                Candidate = x.Candidate.FullName,
                Status = x.Status,
                Vacancy = x.Vacancy.Name,
                Date = x.Date.ToShortDateString()
            })
            .ToListAsync(cancellationToken);

        return (items, count);
    }
}