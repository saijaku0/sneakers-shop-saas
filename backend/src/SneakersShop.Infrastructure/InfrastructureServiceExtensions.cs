using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

using SneakersShop.Application.Abstractions.Authentication;
using SneakersShop.Application.Abstractions.Repositories;
using SneakersShop.Infrastructure.Persistence;
using SneakersShop.Infrastructure.Persistence.Auth;
using SneakersShop.Infrastructure.Persistence.Auth.Abstractions;
using SneakersShop.Infrastructure.Persistence.Identity;
using SneakersShop.Infrastructure.Persistence.Repositories;

namespace SneakersShop.Infrastructure;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        services.AddDbContext<AppDbContext>((sp, options) =>
            options.UseSqlServer(connectionString, b => b.MigrationsAssembly("SneakersShop.Migrations")));

        services.AddSingleton(TimeProvider.System);

        services
            .AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<AppDbContext>();

        // Register repositories and unit of work
        services.AddScoped<IWarehouseItemRepository, WarehouseItemRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // JWT settings — bound once into a typed object, single source of truth
        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>()
            ?? throw new InvalidOperationException("JwtSettings section is missing or invalid.");

        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IJwtService, JwtService>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        });

        return services;
    }
}