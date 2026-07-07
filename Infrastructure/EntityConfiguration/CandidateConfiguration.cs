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
            .Property(x => x.FullName)
            .HasMaxLength(50)
            .IsRequired();
        builder
            .Property(x => x.City)
            .HasMaxLength(100)
            .IsRequired();
        builder
            .Property(x => x.Status)
            .HasMaxLength(50)
            .HasConversion<string>()
            .IsRequired();
        builder
            .Property(x => x.Education)
            .HasMaxLength(20) // максимальное количество элементов массива
            .IsRequired(); // если нет образования - пустой массив
        builder
            .PrimitiveCollection(x => x.Education)
            .ElementType()
            .HasMaxLength(200); // максимальная длина элементов массива
        builder
            .Property(x => x.PreviousWork)
            .HasMaxLength(20) // максимальное количество элементов массива
            .IsRequired(); // если нет работы - пустой массив
        builder
            .PrimitiveCollection(x => x.PreviousWork)
            .ElementType()
            .HasMaxLength(200); // максимальная длина элементов массива
        builder
            .Property(x => x.Phone)
            .HasMaxLength(50)
            .IsRequired();
        builder
            .Property(x => x.Email)
            .HasMaxLength(50)
            .IsRequired();
        builder
            .Property(x => x.Telegram)
            .HasMaxLength(50);
        builder
            .Property(x => x.Skills)
            .HasMaxLength(20) // максимальное количество элементов массива
            .IsRequired();
        builder
            .PrimitiveCollection(x => x.Skills)
            .ElementType()
            .HasMaxLength(200); // максимальная длина элементов массива
    }
}
