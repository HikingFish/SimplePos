using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimplePos.Domain.Sales;
using SimplePos.Domain.Taxes;

namespace SimplePos.Infrastructure.Persistence.Sales;

public class SaleItemTaxConfiguration : IEntityTypeConfiguration<SaleItemTax>
{
    public void Configure(EntityTypeBuilder<SaleItemTax> builder)
    {
        builder.ToTable("sale_item_taxes");

        builder.HasKey(sit => sit.SaleItemTaxId);

        builder.HasOne<Tax>()
            .WithMany()
            .HasForeignKey(sit => sit.TaxId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(sit => sit.TaxName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(sit => sit.TaxRate)
            .HasPrecision(6, 4)
            .IsRequired();

        builder.Property(sit => sit.TaxAmount)
            .HasPrecision(14, 2)
            .IsRequired();
    }
}
