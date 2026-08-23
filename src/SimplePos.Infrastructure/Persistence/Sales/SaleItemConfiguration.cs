using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimplePos.Domain.Products;
using SimplePos.Domain.Sales;
using SimplePos.Domain.Users;

namespace SimplePos.Infrastructure.Persistence.Sales;

public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.ToTable("sale_items");

        builder.HasKey(si => si.SaleItemId);

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(si => si.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(si => si.AddedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(si => si.DateTimeAdded)
            .IsRequired();

        builder.Property(si => si.Void)
            .IsRequired();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(si => si.VoidedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(si => si.DateTimeVoided);

        builder.Property(si => si.Quantity)
            .HasPrecision(14, 4)
            .IsRequired();

        builder.Property(si => si.UnitPrice)
            .HasPrecision(14, 2)
            .IsRequired();

        builder.Property(si => si.GrossAmount)
            .HasPrecision(14, 2)
            .IsRequired();

        builder.Property(si => si.UnitDiscount)
            .HasPrecision(14, 4)
            .IsRequired();

        builder.Property(si => si.DiscountedUnitPrice)
            .HasPrecision(14, 2)
            .IsRequired();

        builder.Property(si => si.NetAmount)
            .HasPrecision(14, 2)
            .IsRequired();

        builder.Property(si => si.TotalDiscount)
            .HasPrecision(14, 2)
            .IsRequired();

        builder.Property(si => si.TotalLineAmount)
            .HasPrecision(14, 2)
            .IsRequired();

        builder.Property(si => si.Remark)
            .HasMaxLength(500);

        builder.Property(si => si.TaxRate)
            .HasPrecision(6, 4)
            .IsRequired();

        builder.Property(si => si.TaxAmount)
            .HasPrecision(14, 2)
            .IsRequired();

        // Relationship with SaleItemTax
        builder.HasMany(si => si.SaleItemTaxes)
            .WithOne()
            .HasForeignKey(sit => sit.SaleItemId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure backing field access mode for _saleItemTaxes
        builder.Navigation(si => si.SaleItemTaxes)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
