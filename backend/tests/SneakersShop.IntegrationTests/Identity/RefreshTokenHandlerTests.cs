using FluentAssertions;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using SneakersShop.Application.Auth.Commands.LoginUser;
using SneakersShop.Application.Auth.Commands.Refresh;
using SneakersShop.Application.Auth.Commands.RegisterNewUser;
using SneakersShop.Infrastructure.Persistence;
using SneakersShop.IntegrationTests.Infrastructure;

namespace SneakersShop.IntegrationTests.Identity;

[Collection("IntegrationTests")]
public class RefreshTokenHandlerTests(DatabaseFixture fixture)
{
    private readonly IServiceProvider _serviceProvider = TestServiceProviderFactory.Build(fixture.ConnectionString);

    [Fact]
    public async Task Handle_WithValidToken_ShouldRotateRefreshTokens()
    {
        var email = $"user_{Guid.NewGuid()}@example.com";
        using var scope = _serviceProvider.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await sender.Send(new RegisterCommand(
            Name: "John",
            LastName: "Doe",
            PhoneNumber: "+491234567890",
            Email: email,
            Password: "Password!123",
            DefaultShippingAddress: null
        ));
        var loginResult = await sender.Send(new LoginCommand(email, "Password!123"));
        var tokenR1 = loginResult.Value.RefreshToken;

        var refreshResult = await sender.Send(new RefreshCommand(tokenR1));

        refreshResult.IsSuccess.Should().BeTrue();
        var tokenR2 = refreshResult.Value.RefreshToken;
        tokenR2.Should().NotBe(tokenR1);

        var tokenR1InDb = await db.RefreshTokens.FirstOrDefaultAsync(t => t.Token == tokenR1);
        var tokenR2InDb = await db.RefreshTokens.FirstOrDefaultAsync(t => t.Token == tokenR2);

        tokenR1InDb!.IsRevoked.Should().BeTrue("The old token should be revoked");
        tokenR2InDb!.IsRevoked.Should().BeFalse("The new token should be active");
    }

    [Fact]
    public async Task Handle_WhenTokenIsReused_ShouldRevokeAllUserTokens()
    {
        var email = $"user_{Guid.NewGuid()}@example.com";
        using var scope = _serviceProvider.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await sender.Send(new RegisterCommand(
            Name: "John",
            LastName: "Doe",
            PhoneNumber: "+491234567890",
            Email: email,
            Password: "Password!123",
            DefaultShippingAddress: null
        ));
        var loginResult = await sender.Send(new LoginCommand(email, "Password!123"));

        var tokenR1 = loginResult.Value.RefreshToken;

        await sender.Send(new RefreshCommand(tokenR1));

        var breachResult = await sender.Send(new RefreshCommand(tokenR1));

        breachResult.IsFailure.Should().BeTrue();
        breachResult.Error.Code.Should().Be("auth.breach_detected");

        var identityUser = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
        identityUser.Should().NotBeNull();
        var userId = identityUser!.Id;

        var userTokens = await db.RefreshTokens
            .Where(t => t.UserId == userId)
            .ToListAsync();

        userTokens.Should().NotBeEmpty();
        userTokens.All(t => t.IsRevoked).Should().BeTrue("All tokens in the token chain should be revoked in case of token theft");
    }
}