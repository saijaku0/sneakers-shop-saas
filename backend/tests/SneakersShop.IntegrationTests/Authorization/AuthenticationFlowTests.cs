using System.Net.Http.Headers;
using System.Text.Json;

using FluentAssertions;

using SneakersShop.IntegrationTests.Infrastructure;

namespace SneakersShop.IntegrationTests.Authorization;

public class AuthenticationFlowTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly TestAuthHelper _authHelper;

    public AuthenticationFlowTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _authHelper = new TestAuthHelper(_client);
    }

    [Fact]
    public async Task CompleteFlow_RegisterLoginAndAccessProtectedResource_Succeeds()
    {
        var registerResponse = await _authHelper.RegisterUserAsync();
        registerResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var loginResponse = await _authHelper.LoginUserAsync();
        loginResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(loginContent);
        var token = document.RootElement.GetProperty("accessToken").GetString();

        var fakeId = Guid.NewGuid();
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/warehouseitems/{fakeId}/reserve");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var finalResponse = await _client.SendAsync(request);

        finalResponse.StatusCode.Should().NotBe(System.Net.HttpStatusCode.Unauthorized);
    }
}