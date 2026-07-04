using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.ModelConfig;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);

        builder
            .Property(x => x.Role)
            .HasMaxLength(100)
            .IsRequired();
        builder
            .Property(x => x.FirstName)
            .HasMaxLength(50)
            .IsRequired();
        builder
            .Property(x => x.LastName)
            .HasMaxLength(50)
            .IsRequired();
        builder
            .Property(x => x.PasswordHash)
            .HasMaxLength(300)
            .IsRequired();
        builder
            .Property(x => x.CreatedAt)
            .HasDefaultValueSql("CURRENT_DATE")
            .IsRequired();
        builder // null (может быть не удален пока что)
            .Property(x => x.RevokedAt);
        builder // null (по той же причине)
            .Property(x => x.RevokedBy);
    }
}
