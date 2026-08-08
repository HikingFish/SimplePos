using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimplePos.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using SimplePos.Domain.Companies;
using SimplePos.Infrastructure.Persistence.Companies;
using SimplePos.Domain.Outlets;
using SimplePos.Infrastructure.Persistence.Outlets;
using SimplePos.Domain.Users;
using SimplePos.Infrastructure.Persistence.Users;
using SimplePos.Domain.Permissions;
using SimplePos.Infrastructure.Persistence.Permissions;
using SimplePos.Application.Abstractions;
using SimplePos.Application.Abstractions.Identity;
using SimplePos.Infrastructure.Identity;

namespace SimplePos.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options => 
        options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

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

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<IOutletRepository, OutletRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<ITokenProvider, TokenProvider>();
        services.AddScoped<IIdentityService, IdentityService>();

        return services;
    }
}