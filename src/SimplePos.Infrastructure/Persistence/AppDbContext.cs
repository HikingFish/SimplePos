using Microsoft.EntityFrameworkCore;
using SimplePos.Domain.Categories;
using SimplePos.Domain.Companies;
using SimplePos.Domain.Outlets;
using SimplePos.Domain.Payments;
using SimplePos.Domain.Permissions;
using SimplePos.Domain.Products;
using SimplePos.Domain.Sales;
using SimplePos.Domain.Taxes;
using SimplePos.Domain.Users;

namespace SimplePos.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Outlet> Outlets => Set<Outlet>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Tax> Taxes => Set<Tax>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();
    public DbSet<SalePayment> SalePayments => Set<SalePayment>();
    public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();
    public DbSet<Permission> Permissions => Set<Permission>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}