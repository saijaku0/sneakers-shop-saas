using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SneakersShop.Application.Abstractions.Authentication;
using SneakersShop.Application.Abstractions.Repositories;
using SneakersShop.Infrastructure.Persistence;
using SneakersShop.Infrastructure.Persistence.Auth;
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

        // Register repositories and unit of work
        services.AddScoped<IWarehouseItemRepository, WarehouseItemRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        //Register jwt settings and service
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        services.AddScoped<IAuthenticationService, AuthenticationService>();

        return services;
    }
}