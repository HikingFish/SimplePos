using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimplePos.Domain.Categories;
using SimplePos.Domain.Companies;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimplePos.Infrastructure.Persistence.Categories;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");

        builder.HasKey(c => c.CategoryId);

        builder.HasOne<Company>()
            .WithMany()
            .HasForeignKey(c => c.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(c => c.Name);
        builder.Property(c => c.IsActive)
            .IsRequired();
        builder.Property(c => c.SoftDeleted)
            .IsRequired();
        builder.Property(c => c.DateTimeSoftDeleted);
    }
}