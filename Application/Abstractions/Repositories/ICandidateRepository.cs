using Application.Interview.DTO;
using Domain.Models;

namespace Application.Abstractions.Repositories;

public interface ICandidateRepository
{
    Task AddAsync(Candidate candidate, CancellationToken cancellationToken);

    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken);

    Task<CandidateResponse?> GetByIdDetailedAsync(int id, CancellationToken cancellationToken);
}