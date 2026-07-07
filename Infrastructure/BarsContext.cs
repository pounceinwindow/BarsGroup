using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure;

// Postgre
public class BarsContext : DbContext
{
    public BarsContext(DbContextOptions<BarsContext> options) : base(options)
    {
    }

    public DbSet<Candidate> Candidates { get; set; }
    public DbSet<Competency> Competitions { get; set; }
    public DbSet<CompetitionsMatrix> CompetitionsMatrix { get; set; }
    public DbSet<Interview> Interviews { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Vacancy> Vacancies { get; set; }
    public DbSet<VacancyCompetition> VacancyCompetition { get; set; }
    public DbSet<Verdict> Verdicts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BarsContext).Assembly);
    }

    public async Task ClearAndSeed()
    {
        await DbSeeder.ClearAndSeedAsync(this);
    }
}
