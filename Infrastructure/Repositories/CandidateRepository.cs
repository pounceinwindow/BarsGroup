using Application.Abstractions.Repositories;
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
}