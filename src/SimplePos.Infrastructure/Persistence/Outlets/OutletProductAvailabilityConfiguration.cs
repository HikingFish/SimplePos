using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimplePos.Domain.Outlets;
using SimplePos.Domain.Products;
using SimplePos.Domain.Users;

namespace SimplePos.Infrastructure.Persistence.Outlets;

public class OutletProductAvailabilityConfiguration : IEntityTypeConfiguration<OutletProductAvailability>
{
    public void Configure(EntityTypeBuilder<OutletProductAvailability> builder)
    {
        builder.ToTable("outlet_product_availabilities");

        builder.HasKey(opa => new { opa.OutletId, opa.ProductId });

        builder.Property(opa => opa.IsAvailable)
            .IsRequired();

        builder.Property(opa => opa.MarkedUnavailableAt);

        builder.Property(opa => opa.MarkedByUserId);

        builder.HasOne<Outlet>()
            .WithMany()
            .HasForeignKey(opa => opa.OutletId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(opa => opa.ProductId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(opa => opa.MarkedByUserId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);
    }
}
