using Domain.Models;

namespace Application.Abstractions.Repositories;

public interface ICandidateRepository
{
    void Add(Candidate candidate);

    Task<bool> ExistsByEmailAsync(string email);
}