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
    public DbSet<Competency> Competencies { get; set; }
    public DbSet<CompetencyMatrix> CompetencyMatrices { get; set; }
    public DbSet<Interview> Interviews { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Vacancy> Vacancies { get; set; }
    public DbSet<VacancyCompetency> VacancyCompetencies { get; set; }
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
