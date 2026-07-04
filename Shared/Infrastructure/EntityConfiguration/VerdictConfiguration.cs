using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.ModelConfiguration;

public class VerdictConfiguration : IEntityTypeConfiguration<Verdict>
{
    public void Configure(EntityTypeBuilder<Verdict> builder)
    {
        builder.HasKey(x => new { x.InterviewId, x.UserId });

        builder
            .HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict); // При  удалении пользователя - ошибка (за ним привязаны решения)
        builder // [TODO: Андрей] - Гонка за решением собеседования
                // пока решение может приянть первый решала, остальные не могут
            .HasOne(x => x.Interview)
            .WithOne()
            .HasForeignKey<Verdict>(x => x.InterviewId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Property(x => x.Decision)
            .IsRequired();
        builder // null (может не оставить комментарий и просто принять)
                // нет ограничений размера
            .Property(x => x.Comment);
    }
}
