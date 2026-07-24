using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimplePos.Domain.Outlets;
using SimplePos.Domain.Sales;

namespace SimplePos.Infrastructure.Persistence.Sales;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("sales");

        builder.HasKey(s => s.SaleId);

        builder.HasOne<Outlet>()
            .WithMany()
            .HasForeignKey(s => s.OutletId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(s => s.InvoiceNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(s => s.InvoiceNumber);

        builder.Property(s => s.SubTotal)
            .HasPrecision(14, 2);

        builder.Property(s => s.TaxAmount)
            .HasPrecision(14, 2);

        builder.Property(s => s.NetAmount)
            .HasPrecision(14, 2);

        builder.Property(s => s.TotalAmount)
            .HasPrecision(14, 2);

        builder.Property(s => s.DateTimeCreated)
            .IsRequired();

        builder.Property(s => s.SoftDeleted)
            .IsRequired();

        builder.Property(s => s.DateTimeSoftDeleted);

        builder.Property(s => s.Void)
            .IsRequired();

        builder.Property(s => s.TotalPaid)
            .HasPrecision(14, 2);

        builder.Property(s => s.TotalChange)
            .HasPrecision(14, 2);

        builder.Property(s => s.TotalOutstanding)
            .HasPrecision(14, 2);

        builder.Property(s => s.TotalDiscount)
            .HasPrecision(14, 2);

        // Relationships
        builder.HasMany(s => s.SaleItems)
            .WithOne()
            .HasForeignKey(si => si.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.SalePayments)
            .WithOne()
            .HasForeignKey(sp => sp.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Property access mode for backing fields (_saleItems, _salePayments)
        builder.Navigation(s => s.SaleItems)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(s => s.SalePayments)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // Global query filter for soft delete
        builder.HasQueryFilter(s => !s.SoftDeleted);

        // Ignore domain event collection from AggregateRoot
        builder.Ignore(s => s.DomainEvents);
    }
}