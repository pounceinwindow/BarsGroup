using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure;

// Postgre
public class AppContext : DbContext
{
    public DbSet<Candidate> Candidates { get; set; }
    public DbSet<Competition> Competitions { get; set; }
    public DbSet<CompetitionsMatrix> CompetitionsMatrix { get; set; }
    public DbSet<Interview> Interviews { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Vacancy> Vacancies { get; set; }
    public DbSet<VacancyCompetition> VacancyCompetition { get; set; }
    public DbSet<Verdict> Verdicts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Кандидат
        modelBuilder.Entity<Candidate>(candidate =>
        {
            candidate.HasKey(x => x.Id);

            candidate
                .Property(x => x.FirstName)
                .HasMaxLength(50)
                .IsRequired();
            candidate
                .Property(x => x.LastName)
                .HasMaxLength(50)
                .IsRequired();
            candidate // null (мог не иметь работы)
                .Property(x => x.PreviousWork)
                .HasMaxLength(500);
            candidate
                .Property(x => x.City)
                .HasMaxLength(100)
                .IsRequired();
            candidate
                .Property(x => x.Status)
                .HasMaxLength(50)
                .IsRequired();
            candidate // null (может быть без образования)
                .Property(x => x.Education)
                .HasMaxLength(500);
            candidate
                .Property(x => x.Phone)
                .HasMaxLength(50)
                .IsRequired();
        });

        modelBuilder.Entity<Competition>(competition =>
        {
            competition.HasKey(x => x.Id);

            competition
                .Property(x => x.Name)
                .HasMaxLength(200)
                .IsRequired();
            competition // размер не ограничен, может быть без описания
                .Property(x => x.Description);
        });

        modelBuilder.Entity<Vacancy>(vacancy =>
        {
            vacancy.HasKey(x => x.Id);

            vacancy
                .Property(x => x.Name)
                .HasMaxLength(500)
                .IsRequired();
        });

        modelBuilder.Entity<User>(user =>
        {
            user.HasKey(x => x.Id);

            user
                .Property(x => x.Role)
                .HasMaxLength(100)
                .IsRequired();
            user
                .Property(x => x.FirstName)
                .HasMaxLength(50)
                .IsRequired();
            user
                .Property(x => x.LastName)
                .HasMaxLength(50)
                .IsRequired();
            user
                .Property(x => x.PasswordHash)
                .HasMaxLength(300)
                .IsRequired();
            user
                .Property(x => x.CreatedAt)
                .HasDefaultValueSql("CURRENT_DATE")
                .IsRequired();
            user // null (может быть не удален пока что)
                .Property(x => x.RevokedAt);
            user // null (по той же причине)
                .Property(x => x.RevokedBy);
        });

        modelBuilder.Entity<Interview>(interview =>
        {
            interview.HasKey(x => x.Id);

            interview // FK
                .HasOne<Vacancy>(x => x.Vacancy)
                .WithMany()
                .HasForeignKey(x => x.VacancyId)
                .OnDelete(DeleteBehavior.Cascade);
            interview // FK
                .HasOne<Candidate>(x => x.Candidate)
                .WithMany()
                .HasForeignKey(x => x.CandidateId)
                .OnDelete(DeleteBehavior.Restrict); // Сохранять интервью удаленного кандидата?
            interview
                .Property(x => x.Date)
                .IsRequired();
            interview
                .Property(x => x.Status)
                .HasMaxLength(100)
                .IsRequired();
        });

        modelBuilder.Entity<Verdict>(verdict =>
        {
            verdict.HasKey(x => new { x.InterviewId, x.UserId });

            verdict
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict); // При  удалении пользователя - ошибка (за ним привязаны решения)
            verdict // [TODO: Андрей] - Гонка за решением собеседования
                    // пока решение может приянть первый решала, остальные не могут
                .HasOne(x => x.Interview)
                .WithOne()
                .HasForeignKey<Verdict>(x => x.InterviewId)
                .OnDelete(DeleteBehavior.Cascade);

            verdict
                .Property(x => x.Decision)
                .IsRequired();
            verdict // null (может не оставить комментарий и просто принять)
                    // нет ограничений размера
                .Property(x => x.Comment); 
        });

        modelBuilder.Entity<VacancyCompetition>(vc =>
        {
            vc.HasKey(x => new { x.VacancyId, x.CompetitionId });

            vc.HasOne(x => x.Vacancy)
                .WithMany()
                .HasForeignKey(x => x.VacancyId)
                .OnDelete(DeleteBehavior.Cascade);

            vc.HasOne(x => x.Competition)
                .WithMany()
                .HasForeignKey(x => x.CompetitionId)
                .OnDelete(DeleteBehavior.ClientCascade);
        });

        modelBuilder.Entity<CompetitionsMatrix>(cm =>
        {
            cm.HasKey(x => new { x.CandidateId, x.CompetitionId });

            cm.HasOne(x => x.Candidate)
                .WithMany()
                .HasForeignKey(x => x.CandidateId)
                .OnDelete(DeleteBehavior.Cascade);
            cm.HasOne(x => x.Competition)
                .WithMany()
                .HasForeignKey(x => x.CompetitionId)
                .OnDelete(DeleteBehavior.Cascade);

            cm.Property(x => x.Score)
                .IsRequired();
            cm.Property(x => x.Comment); // не обязателен
        });
    }
}
