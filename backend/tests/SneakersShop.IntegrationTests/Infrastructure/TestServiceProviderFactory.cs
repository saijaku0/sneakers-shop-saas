using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SneakersShop.Application;
using SneakersShop.Infrastructure;

namespace SneakersShop.IntegrationTests.Infrastructure;

public static class TestServiceProviderFactory
{
    public static IServiceProvider Build(string connectionString)
    {
        ServiceCollection services = new();
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings:DefaultConnection", connectionString },
                { "JwtSettings:Secret", "SuperSecretKeyForTestingPurposesOnly12345!_@" },
                { "JwtSettings:Issuer", "TestIssuer" },
                { "JwtSettings:Audience", "TestAudience" },
                { "JwtSettings:ExpiryMinutes", "60" }
            })
            .Build();

        services.AddSingleton<IConfiguration>(configuration);

        services.AddLogging();

        services.AddApplication();
        services.AddInfrastructure(configuration);

        return services.BuildServiceProvider();
    }
}