using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.ModelConfiguration;

public class CandidateConfiguration : IEntityTypeConfiguration<Candidate>
{
    public void Configure(EntityTypeBuilder<Candidate> builder)
    {
        builder.HasKey(x => x.Id);

        builder
            .Property(x => x.FirstName)
            .HasMaxLength(50)
            .IsRequired();
        builder
            .Property(x => x.LastName)
            .HasMaxLength(50)
            .IsRequired();
        builder // null (мог не иметь работы)
            .Property(x => x.PreviousWork)
            .HasMaxLength(500);
        builder
            .Property(x => x.City)
            .HasMaxLength(100)
            .IsRequired();
        builder
            .Property(x => x.Status)
            .HasMaxLength(50)
            .IsRequired();
        builder // null (может быть без образования)
            .Property(x => x.Education)
            .HasMaxLength(500);
        builder
            .Property(x => x.Phone)
            .HasMaxLength(50)
            .IsRequired();
    }
}
