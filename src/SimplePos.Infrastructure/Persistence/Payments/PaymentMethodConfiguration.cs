using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimplePos.Domain.Companies;
using SimplePos.Domain.Payments;

namespace SimplePos.Infrastructure.Persistence.Payments;

public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> builder)
    {
        builder.ToTable("payment_methods");

        builder.HasKey(pm => pm.PaymentMethodId);

        builder.Property(pm => pm.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(pm => pm.IsActive)
            .IsRequired();

        builder.Property(pm => pm.SoftDeleted)
            .IsRequired();

        builder.Property(pm => pm.DateTimeSoftDeleted);

        builder.HasOne<Company>()
            .WithMany()
            .HasForeignKey(pm => pm.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(pm => !pm.SoftDeleted);
    }
}