using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimplePos.Domain.Categories;
using SimplePos.Domain.Companies;
using SimplePos.Domain.Products;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimplePos.Infrastructure.Persistence.Products;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");

        builder.HasKey(p => p.ProductId);

        builder.HasOne<Company>()
            .WithMany()
            .HasForeignKey(p => p.CompanyId)
            .IsRequired();

        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(p => p.CategoryId)
            .IsRequired();

        builder.Property(p => p.SKU)
            .HasMaxLength(50);

        builder.Property(p => p.ProductName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.CostPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.BasePrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.IsActive)
            .IsRequired();

        builder.Property(p => p.SoftDeleted)
            .IsRequired();

        builder.Property(p => p.DateTimeSoftDeleted);

        builder.Metadata.FindNavigation(nameof(Product.ProductTaxes))?
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}