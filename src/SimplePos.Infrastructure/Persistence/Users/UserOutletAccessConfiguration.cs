using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimplePos.Domain.Outlets;
using SimplePos.Domain.Users;

namespace SimplePos.Infrastructure.Persistence.Users;

public class UserOutletAccessConfiguration : IEntityTypeConfiguration<UserOutletAccess>
{
    public void Configure(EntityTypeBuilder<UserOutletAccess> builder)
    {
        builder.ToTable("user_outlet_accesses");

        builder.HasKey(uoa => new { uoa.UserId, uoa.OutletId });

        builder.HasOne<User>()
            .WithMany(u => u.UserOutletAccesses)
            .HasForeignKey(uoa => uoa.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Outlet>()
            .WithMany()
            .HasForeignKey(uoa => uoa.OutletId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
