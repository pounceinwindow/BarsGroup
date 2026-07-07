using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.ModelConfiguration;

public class InterviewConfiguration : IEntityTypeConfiguration<Interview>
{
    public void Configure(EntityTypeBuilder<Interview> builder)
    {
        builder.HasKey(x => x.Id);

        builder // FK
            .HasOne(x => x.Vacancy)
            .WithMany()
            .HasForeignKey(x => x.VacancyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder // FK
            .HasOne(x => x.Candidate)
            .WithMany(x => x.Interviews)
            .HasForeignKey(x => x.CandidateId)
            .OnDelete(DeleteBehavior.Restrict); // Сохранять интервью удаленного кандидата?

        builder
            .Property(x => x.ProcessId)
            .IsRequired();

        builder
            .Property(x => x.Date)
            .IsRequired();

        builder
            .Property(x => x.Status)
            .HasMaxLength(100)
            .HasConversion<string>()
            .IsRequired();

        builder
            .Property(x => x.SummaryComment)
            .IsRequired(false);

        builder
            .HasMany(x => x.MatrixRows)
            .WithOne(x => x.Interview)
            .HasForeignKey(x => x.InterviewId)
            .OnDelete(DeleteBehavior.Cascade);

        // MatrixRows в домене - IReadOnlyCollection поверх приватного List _matrixRows.
        // EF не может писать в read-only свойство напрямую, поэтому указываем backing field:
        // загрузка из БД и change tracking идут через _matrixRows, снаружи виден только getter.
        builder.Navigation(x => x.MatrixRows)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Metadata.FindNavigation(nameof(Interview.MatrixRows))!
            .SetField("_matrixRows");
    }
}