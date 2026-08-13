using FluentAssertions;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

using SneakersShop.Application.Auth.Commands.LoginUser;
using SneakersShop.Application.Auth.Commands.RegisterNewUser;
using SneakersShop.IntegrationTests.Infrastructure;

namespace SneakersShop.IntegrationTests.Auth;

[Collection("IntegrationTests")]
public class LoginUserHandlerTests(DatabaseFixture fixture)
{
    private readonly IServiceProvider _serviceProvider = TestServiceProviderFactory.Build(fixture.ConnectionString);

    [Fact]
    public async Task Handle_WithInvalidEmailOrPassword_ShouldReturnInvalidCredentialsError()
    {
        var email = $"user_{Guid.NewGuid()}@example.com";
        var password = "Password123!";

        using var scope = _serviceProvider.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();

        await sender.Send(new RegisterCommand(
            Name: "John",
            LastName: "Doe",
            PhoneNumber: "+491234567890",
            Email: email,
            Password: password,
            DefaultShippingAddress: null
        ));

        var unknownEmailResult = await sender.Send(new LoginCommand("fake@test.com", password));

        var badPasswordResult = await sender.Send(new LoginCommand(email, "WrongPass123!"));

        unknownEmailResult.IsFailure.Should().BeTrue();
        badPasswordResult.IsFailure.Should().BeTrue();

        unknownEmailResult.Error.Code.Should().Be(badPasswordResult.Error.Code);
        unknownEmailResult.Error.Message.Should().Be(badPasswordResult.Error.Message);
    }
}