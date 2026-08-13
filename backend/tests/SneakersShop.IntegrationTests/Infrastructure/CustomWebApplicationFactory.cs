using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SneakersShop.Infrastructure.Persistence;

using Testcontainers.MsSql;

namespace SneakersShop.IntegrationTests.Infrastructure;

public class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram>, IAsyncLifetime where TProgram : class
{
    private readonly MsSqlContainer _container = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
        .WithPassword("Password112233!")
        .Build();

    public CustomWebApplicationFactory()
    {
        Environment.SetEnvironmentVariable("JwtSettings__SecretKey", "SuperSecretKeyForTestingPurposesOnly12345!_@");
        Environment.SetEnvironmentVariable("JwtSettings__Issuer", "TestIssuer");
        Environment.SetEnvironmentVariable("JwtSettings__Audience", "TestAudience");
        Environment.SetEnvironmentVariable("JwtSettings__AccessTokenExpiryMinutes", "60");
        Environment.SetEnvironmentVariable("JwtSettings__RefreshTokenExpiryDays", "7");
    }

    public string ConnectionString => _container.GetConnectionString() + ";Database=SneakersShopDb;TrustServerCertificate=true";

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        await ApplyMigrationsAsync();
    }

    async Task IAsyncLifetime.DisposeAsync() => await _container.StopAsync();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (dbContextDescriptor is not null)
                services.Remove(dbContextDescriptor);

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(ConnectionString, b => b.MigrationsAssembly("SneakersShop.Migrations")));

        });
    }

    private async Task ApplyMigrationsAsync()
    {
        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(ConnectionString, b => b.MigrationsAssembly("SneakersShop.Migrations"))
            .Options;

        await using var context = new AppDbContext(options);
        await context.Database.MigrateAsync();
    }
}