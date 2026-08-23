using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimplePos.Domain.Payments;
using SimplePos.Domain.Sales;
using SimplePos.Domain.Users;

namespace SimplePos.Infrastructure.Persistence.Sales;

public class SalePaymentConfiguration : IEntityTypeConfiguration<SalePayment>
{
    public void Configure(EntityTypeBuilder<SalePayment> builder)
    {
        builder.ToTable("sale_payments");

        builder.HasKey(sp => sp.SalePaymentId);

        builder.HasOne<PaymentMethod>()
            .WithMany()
            .HasForeignKey(sp => sp.PaymentMethodId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(sp => sp.ProcessedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(sp => sp.AmountPaid)
            .HasPrecision(14, 2)
            .IsRequired();

        builder.Property(sp => sp.PaymentDate)
            .IsRequired();

        builder.Property(sp => sp.ReferenceNumber)
            .HasMaxLength(100);
    }
}
