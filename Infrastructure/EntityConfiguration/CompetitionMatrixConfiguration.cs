using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Infrastructure.ModelConfiguration;

public class CompetitionMatrixConfiguration : IEntityTypeConfiguration<CompetitionsMatrix>
{
    public void Configure(EntityTypeBuilder<CompetitionsMatrix> builder)
    {
        builder.HasKey(x => new { x.CandidateId, x.CompetitionId });

        builder.HasOne(x => x.Candidate)
            .WithMany()
            .HasForeignKey(x => x.CandidateId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Competition)
            .WithMany()
            .HasForeignKey(x => x.CompetitionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.Score)
            .IsRequired();
        builder.Property(x => x.Comment); // не обязателен
    }
}
