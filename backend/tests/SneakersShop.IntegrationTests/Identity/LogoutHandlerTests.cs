using FluentAssertions;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

using SneakersShop.Application.Auth.Commands.LoginUser;
using SneakersShop.Application.Auth.Commands.LogoutUser;
using SneakersShop.Application.Auth.Commands.RegisterNewUser;
using SneakersShop.IntegrationTests.Infrastructure;

namespace SneakersShop.IntegrationTests.Identity;

[Collection("IntegrationTests")]
public class LogoutHandlerTests(DatabaseFixture fixture)
{
    private readonly IServiceProvider _serviceProvider = TestServiceProviderFactory.Build(fixture.ConnectionString);

    [Fact]
    public async Task Handle_ShouldBeIdempotent()
    {
        var email = $"user_{Guid.NewGuid()}@example.com";
        using var scope = _serviceProvider.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();

        await sender.Send(new RegisterCommand(
            Name: "John",
            LastName: "Doe",
            PhoneNumber: "+491234567890",
            Email: email,
            Password: "Password!123",
            DefaultShippingAddress: null
        ));
        var loginResult = await sender.Send(new LoginCommand(email, "Password!123"));
        var validToken = loginResult.Value.RefreshToken;

        var logoutResult1 = await sender.Send(new LogoutUserCommand(validToken));
        logoutResult1.IsSuccess.Should().BeTrue();

        var logoutResult2 = await sender.Send(new LogoutUserCommand(validToken));
        var logoutResult3 = await sender.Send(new LogoutUserCommand("some-garbage-token-string"));

        logoutResult2.IsSuccess.Should().BeTrue("Repeated logout should return Success (idempotency)");
        logoutResult3.IsSuccess.Should().BeTrue("Logout with a non-existent token should not break the system");
    }
}