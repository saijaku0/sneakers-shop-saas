namespace SneakersShop.IntegrationTests.Infrastructure;

[CollectionDefinition("IntegrationTestCollection")]
public class IntegrationTestCollection : ICollectionFixture<DatabaseFixture>
{
}