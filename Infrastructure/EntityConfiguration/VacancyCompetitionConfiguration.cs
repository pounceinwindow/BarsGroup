using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.ModelConfiguration;

public class VacancyCompetitionConfiguration : IEntityTypeConfiguration<VacancyCompetency>
{
    public void Configure(EntityTypeBuilder<VacancyCompetency> builder)
    {
        builder.HasKey(x => new { x.VacancyId, x.CompetencyId });

        builder.HasOne(x => x.Vacancy)
            .WithMany()
            .HasForeignKey(x => x.VacancyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Competency)
            .WithMany()
            .HasForeignKey(x => x.CompetencyId)
            .OnDelete(DeleteBehavior.ClientCascade);
    }
}
