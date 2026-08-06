using Microsoft.EntityFrameworkCore;

using SneakersShop.Infrastructure.Persistence;

using Testcontainers.MsSql;

namespace SneakersShop.IntegrationTests.Infrastructure;

public class DatabaseFixture : IAsyncLifetime
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

    public async Task DisposeAsync() =>
        await _container.StopAsync();

    private async Task ApplyMigrationsAsync()
    {
        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(ConnectionString, b => b.MigrationsAssembly("SneakersShop.Migrations"))
            .Options;

        await using var context = new AppDbContext(options);
        await context.Database.MigrateAsync();
    }

    public AppDbContext CreateDbContext()
    {
        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(ConnectionString, b => b.MigrationsAssembly("SneakersShop.Migrations"))
            .Options;
        return new AppDbContext(options);
    }
}