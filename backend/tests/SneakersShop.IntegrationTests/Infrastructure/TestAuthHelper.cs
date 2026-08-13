using System.Net.Http.Json;
using System.Text.Json;

namespace SneakersShop.IntegrationTests.Infrastructure;

public class TestAuthHelper
{
    private readonly HttpClient _client;
    public string TestEmail { get; }
    public string TestPassword { get; } = "Password123!";

    public TestAuthHelper(HttpClient client)
    {
        _client = client;
        TestEmail = $"user_{Guid.NewGuid()}@example.com";
    }

    public async Task<HttpResponseMessage> RegisterUserAsync()
    {
        var registrationData = new
        {
            name = "Ivan",
            lastName = "Petrov",
            phoneNumber = "+491234567890",
            email = TestEmail,
            password = TestPassword,
            defaultShippingAddress = new
            {
                country = "Germany",
                state = (string?)null,
                city = "Nuremberg",
                street = "Hauptstrasse",
                houseNumber = "10",
                zipCode = "90402"
            }
        };

        var registerResponse = await _client.PostAsJsonAsync("/api/v1/auth/register", registrationData);

        if (!registerResponse.IsSuccessStatusCode)
        {
            var error = await registerResponse.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Failed to register user. Status: {registerResponse.StatusCode}. Details: {error}");
        }

        return registerResponse;
    }

    public async Task<HttpResponseMessage> LoginUserAsync()
    {
        var loginRequest = new
        {
            Email = TestEmail,
            Password = TestPassword
        };
        return await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);
    }

    public async Task<string> GetValidAccessTokenAsync()
    {
        await RegisterUserAsync();
        var loginResponse = await LoginUserAsync();

        if (!loginResponse.IsSuccessStatusCode)
        {
            var error = await loginResponse.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Failed to login. Status: {loginResponse.StatusCode}. Details: {error}");
        }

        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(loginContent);
        return document.RootElement.GetProperty("accessToken").GetString()!;
    }
}