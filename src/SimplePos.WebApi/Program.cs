using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using SimplePos.Application;
using SimplePos.Infrastructure;
using SimplePos.Infrastructure.Identity;
using SimplePos.WebApi.Endpoints;
using SimplePos.WebApi.EndPoints;

using SimplePos.WebApi.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

builder.Services.AddApplication();
builder.Services.AddDataProtection(); // <-- Added to provide IDataProtectionProvider for Identity token providers
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)
        )
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.Requirements.Add(new PermissionRequirement("Admin")));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        // 1. Automatically pre-select the "Bearer" scheme in the UI
        options.AddPreferredSecuritySchemes("Bearer");

        // 2. (Optional) Auto-fill / pre-authorize with a default dev JWT token
        options.AddHttpAuthentication("Bearer", auth =>
        {
            // You can load a test token from appsettings or leave empty for manual entry
            auth.Token = builder.Configuration["Jwt:DevToken"] ?? "";
            auth.Description = "Auto-injected JWT Bearer token";
        });

        // 3. Persist the entered token in localStorage across browser page refreshes
        options.EnablePersistentAuthentication();
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapCompanyEndpoints();
app.MapRegisterBusinessEndpoints();
app.MapAuthEndpoints();
app.MapUserOutletAccessEndpoints();
app.MapProductEndpoints();
app.MapCategoryEndpoints();
app.MapTaxEndpoints();
app.MapPaymentMethodEndpoints();
app.MapSaleEndpoints();

app.Run();

