using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimplePos.Application.Abstractions;
using SimplePos.Application.Abstractions.Identity;
using SimplePos.Domain.Categories;
using SimplePos.Domain.Companies;
using SimplePos.Domain.Outlets;
using SimplePos.Domain.Payments;
using SimplePos.Domain.Permissions;
using SimplePos.Domain.Products;
using SimplePos.Domain.Sales;
using SimplePos.Domain.Taxes;
using SimplePos.Domain.Users;
using SimplePos.Infrastructure.Identity;
using SimplePos.Infrastructure.Persistence;
using SimplePos.Infrastructure.Persistence.Categories;
using SimplePos.Infrastructure.Persistence.Companies;
using SimplePos.Infrastructure.Persistence.Outlets;
using SimplePos.Infrastructure.Persistence.Payments;
using SimplePos.Infrastructure.Persistence.Permissions;
using SimplePos.Infrastructure.Persistence.Products;
using SimplePos.Infrastructure.Persistence.Sales;
using SimplePos.Infrastructure.Persistence.Taxes;
using SimplePos.Infrastructure.Persistence.Users;

namespace SimplePos.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options => 
        options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
            options.InstanceName = "SimplePos:";
        });

        services.AddHttpContextAccessor();

        services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = false;
            options.User.RequireUniqueEmail = true;
        })
        .AddSignInManager()
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        services.AddScoped<IPermissionCacheService, PermissionCacheService>();
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<IOutletRepository, OutletRepository>();
        services.AddScoped<IOutletProductAvailabilityRepository, OutletProductAvailabilityRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserPermissionRepository, UserPermissionRepository>();
        services.AddScoped<IUserOutletAccessRepository, UserOutletAccessRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<ITokenProvider, TokenProvider>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<ITaxRepository, TaxRepository>();
        services.AddScoped<ISaleRepository, SaleRepository>();
        services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();

        return services;
    }
}