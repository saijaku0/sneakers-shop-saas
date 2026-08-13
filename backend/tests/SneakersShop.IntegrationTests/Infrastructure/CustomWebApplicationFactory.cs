using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using SneakersShop.Infrastructure.Persistence;
using SneakersShop.Infrastructure.Persistence.Auth;

using Testcontainers.MsSql;

namespace SneakersShop.IntegrationTests.Infrastructure;

public class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram>, IAsyncLifetime where TProgram : class
{
    private readonly MsSqlContainer _container = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
        .WithPassword("Password112233!")
        .Build();

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

            services.Configure<JwtSettings>(opt =>
            {
                opt.SecretKey = "SuperSecretKeyForTestingPurposesOnly12345!_@";
                opt.Issuer = "TestIssuer";
                opt.Audience = "TestAudience";
                opt.AccessTokenExpiryMinutes = 60;
                opt.RefreshTokenExpiryDays = 7;
            });
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