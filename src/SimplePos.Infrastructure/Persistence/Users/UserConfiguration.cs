using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimplePos.Domain.Common;
using SimplePos.Domain.Companies;
using SimplePos.Domain.Outlets;
using SimplePos.Domain.Users;

namespace SimplePos.Infrastructure.Persistence.Users;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.HasKey(u => u.UserId);

        builder.HasOne<Company>()
            .WithMany()
            .HasForeignKey(c => c.CompanyId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasOne<Outlet>()
            .WithMany()
            .HasForeignKey(o => o.OutletId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.Property(u => u.Username).HasMaxLength(200).IsRequired();

        builder.OwnsOne(e => e.Email, emailBuilder =>
        {
            emailBuilder.Property(a => a.Value)
                .HasColumnName("Email")
                .IsRequired()
                .HasMaxLength(256);
        });

        builder.Property(u => u.PhoneNumber).HasMaxLength(20);
        builder.Property(u => u.UserPosition).HasMaxLength(100);
        builder.Property(u => u.IsActive).IsRequired();
        builder.Property(u => u.DateTimeLastLogin);
        builder.Property(u => u.DateTimeCreated).IsRequired();
        builder.Property(u => u.SoftDeleted).IsRequired();
        builder.Property(u => u.DateTimeSoftDeleted);

        builder.Metadata.FindNavigation(nameof(User.UserPermissions))?
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasQueryFilter(u => !u.SoftDeleted);
    }
}