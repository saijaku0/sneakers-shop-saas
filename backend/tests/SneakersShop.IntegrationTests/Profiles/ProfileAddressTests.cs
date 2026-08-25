using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using SneakersShop.Application.Profiles.DTOs;
using SneakersShop.Infrastructure.Persistence;
using SneakersShop.IntegrationTests.Infrastructure;

namespace SneakersShop.IntegrationTests.Profiles;

public class ProfileAddressTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly TestAuthHelper _authHelper;

    public ProfileAddressTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _authHelper = new TestAuthHelper(_client);
    }

    private async Task<string> GetTokenWithoutAddressAsync()
    {
        var email = $"user_{Guid.NewGuid()}@example.com";
        const string password = "Password123!";

        var register = await _client.PostAsJsonAsync("/api/v1/auth/register", new
        {
            name = "Ivan",
            lastName = "Petrov",
            phoneNumber = "+491234567890",
            email,
            password,
            defaultShippingAddress = (object?)null
        });
        register.EnsureSuccessStatusCode();

        var login = await _client.PostAsJsonAsync("/api/v1/auth/login", new { Email = email, Password = password });
        login.EnsureSuccessStatusCode();

        var content = await login.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(content);
        return doc.RootElement.GetProperty("accessToken").GetString()!;
    }

    private HttpRequestMessage BuildGet(string? token)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/profile/address");
        if (token is not null)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return request;
    }

    private HttpRequestMessage BuildPut(string? token, object address)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, "/api/v1/profile/address")
        {
            Content = JsonContent.Create(new { address })
        };
        if (token is not null)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return request;
    }

    private static object SampleAddress(string city = "Nuremberg") => new
    {
        country = "Germany",
        state = (string?)null,
        city,
        street = "Hauptstrasse",
        houseNumber = "10",
        zipCode = "90402"
    };

    [Fact]
    public async Task GetAddress_WhenUserHasDefaultAddress_ReturnsIt()
    {
        var token = await _authHelper.GetValidAccessTokenAsync();

        var response = await _client.SendAsync(BuildGet(token));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<DefaultAddressResponse>();
        body!.Address.Should().NotBeNull();
        body.Address!.City.Should().Be("Nuremberg");
    }

    [Fact]
    public async Task GetAddress_WhenNoDefaultAddress_ReturnsNull()
    {
        var token = await GetTokenWithoutAddressAsync();

        var response = await _client.SendAsync(BuildGet(token));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<DefaultAddressResponse>();
        body!.Address.Should().BeNull();
    }

    [Fact]
    public async Task GetAddress_WithoutToken_Returns401()
    {
        var response = await _client.SendAsync(BuildGet(token: null));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateAddress_SetsDefaultAddress_ThenGetReturnsIt()
    {
        var token = await GetTokenWithoutAddressAsync();

        var put = await _client.SendAsync(BuildPut(token, SampleAddress(city: "Berlin")));
        put.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var get = await _client.SendAsync(BuildGet(token));
        var body = await get.Content.ReadFromJsonAsync<DefaultAddressResponse>();
        body!.Address!.City.Should().Be("Berlin");
    }

    [Fact]
    public async Task UpdateAddress_OverwritesExisting()
    {
        var token = await _authHelper.GetValidAccessTokenAsync();

        var put = await _client.SendAsync(BuildPut(token, SampleAddress(city: "Munich")));
        put.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var get = await _client.SendAsync(BuildGet(token));
        var body = await get.Content.ReadFromJsonAsync<DefaultAddressResponse>();
        body!.Address!.City.Should().Be("Munich");
    }

    [Fact]
    public async Task UpdateAddress_PersistsToDatabase()
    {
        var token = await GetTokenWithoutAddressAsync();

        await _client.SendAsync(BuildPut(token, SampleAddress(city: "Hamburg")));

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var profile = await context.UserProfiles
            .FirstOrDefaultAsync(u => u.DefaultAddress!.City == "Hamburg");
        profile.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateAddress_WithoutToken_Returns401()
    {
        var response = await _client.SendAsync(BuildPut(token: null, SampleAddress()));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}