using System.Net.Http.Headers;

using FluentAssertions;

using SneakersShop.IntegrationTests.Infrastructure;

namespace SneakersShop.IntegrationTests.Warehouse;

public class WarehouseItemsAuthorizationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly TestAuthHelper _authHelper;

    public WarehouseItemsAuthorizationTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _authHelper = new TestAuthHelper(_client);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithoutToken_Returns401()
    {
        var fakeId = Guid.NewGuid();
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/warehouseitems/{fakeId}/reserve");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithValidToken_DoesNotReturn401()
    {
        var token = await _authHelper.GetValidAccessTokenAsync();

        var fakeId = Guid.NewGuid();
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/warehouseitems/{fakeId}/reserve");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().NotBe(System.Net.HttpStatusCode.Unauthorized);
    }
}