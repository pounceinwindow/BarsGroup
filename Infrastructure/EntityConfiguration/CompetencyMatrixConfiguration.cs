using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Infrastructure.ModelConfiguration;

public class CompetencyMatrixConfiguration : IEntityTypeConfiguration<CompetencyMatrix>
{
    public void Configure(EntityTypeBuilder<CompetencyMatrix> builder)
    {
        builder.HasKey(x => new { x.InterviewId, x.CompetencyId });

        builder.HasOne(x => x.Interview)
            .WithMany()
            .HasForeignKey(x => x.InterviewId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Competency)
            .WithMany()
            .HasForeignKey(x => x.CompetencyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.Score)
            .IsRequired();
        builder.Property(x => x.Comment)
            .IsRequired(false); // не обязателен
    }
}
