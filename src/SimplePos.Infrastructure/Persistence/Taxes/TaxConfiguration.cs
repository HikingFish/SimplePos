using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimplePos.Domain.Common;
using SimplePos.Domain.Companies;
using SimplePos.Domain.Taxes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimplePos.Infrastructure.Persistence.Taxes;

public class TaxConfiguration : IEntityTypeConfiguration<Tax>
{
    public void Configure(EntityTypeBuilder<Tax> builder)
    {
        builder.ToTable("taxes");

        builder.HasKey(t => t.TaxId);

        builder.HasOne<Company>()
            .WithMany()
            .HasForeignKey(t => t.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(t => t.TaxName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.TaxRate)
            .HasPrecision(5, 4)
            .IsRequired();

        builder.Property(t => t.IsActive)
            .IsRequired();

        builder.Property(t => t.SoftDeleted)
            .IsRequired();

        builder.Property(t => t.DateTimeSoftDeleted);

    }
}