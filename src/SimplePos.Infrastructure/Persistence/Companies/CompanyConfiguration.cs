using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimplePos.Domain.Companies;

namespace SimplePos.Infrastructure.Persistence.Companies;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        // TABLE NAME
        // By default, EF uses the DbSet property name ("Companies").
        // This makes it explicit.
        builder.ToTable("companies");

        // PRIMARY KEY
        builder.HasKey(c => c.CompanyId);

        // PROPERTIES
        builder.Property(c => c.Name)
            .IsRequired()           // NOT NULL in PostgreSQL
            .HasMaxLength(200);     // VARCHAR(200) — always set a max length

        builder.Property(c => c.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(c => c.IsActive)
            .IsRequired();

        builder.Property(c => c.SoftDeleted)
            .IsRequired();

        // OWNED VALUE OBJECT: Address
        // "Owned" means Address doesn't have its own table — its properties
        // are flattened into the Company table as columns.
        // Company.CompanyAddress.Street → companies.company_address_street
        builder.OwnsOne(c => c.CompanyAddress, addressBuilder =>
        {
            addressBuilder.Property(a => a.Street)
                .HasColumnName("address_street")
                .IsRequired()
                .HasMaxLength(300);

            addressBuilder.Property(a => a.City)
                .HasColumnName("address_city")
                .IsRequired()
                .HasMaxLength(100);

            addressBuilder.Property(a => a.State)
                .HasColumnName("address_state")
                .IsRequired()
                .HasMaxLength(100);

            addressBuilder.Property(a => a.PostalCode)
                .HasColumnName("address_postal_code")
                .IsRequired()
                .HasMaxLength(20);

            addressBuilder.Property(a => a.Country)
                .HasColumnName("address_country")
                .IsRequired()
                .HasMaxLength(100);
        });

        // OWNED VALUE OBJECT: EmailAddress
        // Single-property value object — stored as a column in the Company table.
        builder.OwnsOne(c => c.Email, emailBuilder =>
        {
            emailBuilder.Property(e => e.Value)
                .HasColumnName("email")
                .IsRequired()
                .HasMaxLength(256);
        });

        // GLOBAL QUERY FILTER
        // Every query on Companies will automatically exclude soft-deleted rows.
        // SELECT * FROM companies → SELECT * FROM companies WHERE soft_deleted = false
        // You can bypass this with .IgnoreQueryFilters() when needed.
        builder.HasQueryFilter(c => !c.SoftDeleted);

        // IGNORE domain event collection — this is not persisted.
        builder.Ignore(c => c.DomainEvents);
    }
}
