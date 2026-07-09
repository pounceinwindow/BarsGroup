using Application.Candidate.DTO;
using Application.Interview.DTO;
using Domain.Models;

namespace Application.Abstractions.Repositories;

public interface ICandidateRepository
{
    Task AddAsync(Domain.Models.Candidate candidate, CancellationToken cancellationToken);

    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken);

    Task<bool> ExistsByIdAsync(int id, CancellationToken cancellationToken);

    Task<CandidateResponse?> GetByIdDetailedAsync(int id, CancellationToken cancellationToken);

    Task<(List<CandidateDto> Items, int TotalCount)> GetFilteredCandidates(CandidateFilters filters);

}