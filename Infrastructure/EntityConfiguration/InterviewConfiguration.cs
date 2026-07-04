using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.ModelConfiguration;

public class InterviewConfiguration : IEntityTypeConfiguration<Interview>
{
    public void Configure(EntityTypeBuilder<Interview> builder)
    {
        builder.HasKey(x => x.Id);

        builder // FK
            .HasOne<Vacancy>(x => x.Vacancy)
            .WithMany()
            .HasForeignKey(x => x.VacancyId)
            .OnDelete(DeleteBehavior.Cascade);
        builder // FK
            .HasOne<Candidate>(x => x.Candidate)
            .WithMany()
            .HasForeignKey(x => x.CandidateId)
            .OnDelete(DeleteBehavior.Restrict); // Сохранять интервью удаленного кандидата?
        builder
            .Property(x => x.Date)
            .IsRequired();
        builder
            .Property(x => x.Status)
            .HasMaxLength(100)
            .IsRequired();
    }
}
