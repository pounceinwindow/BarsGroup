using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.ModelConfiguration;

public class VacancyCompetitionConfiguration : IEntityTypeConfiguration<VacancyCompetition>
{
    public void Configure(EntityTypeBuilder<VacancyCompetition> builder)
    {
        builder.HasKey(x => new { x.VacancyId, x.CompetitionId });

        builder.HasOne(x => x.Vacancy)
            .WithMany()
            .HasForeignKey(x => x.VacancyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Competition)
            .WithMany()
            .HasForeignKey(x => x.CompetitionId)
            .OnDelete(DeleteBehavior.ClientCascade);
    }
}
