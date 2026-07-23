using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimplePos.Domain.Products;
using SimplePos.Domain.Taxes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimplePos.Infrastructure.Persistence.Products;

public class ProductTaxConfiguration : IEntityTypeConfiguration<ProductTax>
{
    public void Configure(EntityTypeBuilder<ProductTax> builder)
    {
        builder.ToTable("product_taxes");

        builder.HasKey(pt => new {pt.TaxId, pt.ProductId});

        builder.HasOne<Product>()
            .WithMany(p => p.ProductTaxes)
            .HasForeignKey(p => p.TaxId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Tax>()
            .WithMany()
            .HasForeignKey(pt => pt.TaxId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}