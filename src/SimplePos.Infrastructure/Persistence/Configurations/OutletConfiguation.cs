using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimplePos.Domain.Companies;
using SimplePos.Domain.Outlets;

public class OutletConfiguration : IEntityTypeConfiguration<Outlet>
{
    public void Configure(EntityTypeBuilder<Outlet> builder)
    {
        builder.ToTable("outlets");
        builder.HasKey(o => o.OutletId);

        builder.Property(o => o.Name).IsRequired().HasMaxLength(200);
        builder.Property(o => o.PhoneNumber).HasMaxLength(20);

        // FOREIGN KEY: Outlet belongs to a Company.
        // Even though Outlet doesn't have a navigation property to Company,
        // we can still define the FK relationship for database integrity.
        builder.HasOne<Company>()
            .WithMany()
            .HasForeignKey(o => o.CompanyId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

        builder.OwnsOne(o => o.OutletAddress, addressBuilder =>
        {
            addressBuilder.Property(a => a.Street)
                .HasColumnName("address_street").IsRequired().HasMaxLength(300);
            addressBuilder.Property(a => a.City)
                .HasColumnName("address_city").IsRequired().HasMaxLength(100);
            addressBuilder.Property(a => a.State)
                .HasColumnName("address_state").IsRequired().HasMaxLength(100);
            addressBuilder.Property(a => a.PostalCode)
                .HasColumnName("address_postal_code").IsRequired().HasMaxLength(20);
            addressBuilder.Property(a => a.Country)
                .HasColumnName("address_country").IsRequired().HasMaxLength(100);
        });

        builder.HasQueryFilter(o => !o.SoftDeleted);
    }
}
