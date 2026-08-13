using FluentAssertions;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using SneakersShop.Application.Auth.Commands.RegisterNewUser;
using SneakersShop.Infrastructure.Persistence;
using SneakersShop.IntegrationTests.Infrastructure;

namespace SneakersShop.IntegrationTests.Identity;

[Collection("IntegrationTests")]
public class RegisterUserHandlerTests(DatabaseFixture fixture)
{
    private readonly IServiceProvider _serviceProvider = TestServiceProviderFactory.Build(fixture.ConnectionString);

    [Fact]
    public async Task Handle_ShouldCreateUserAndProfile_WithSameId()
    {
        var email = $"user_{Guid.NewGuid()}@example.com";
        var command = new RegisterCommand(
            Name: "John",
            LastName: "Doe",
            PhoneNumber: "+491234567890",
            Email: email,
            Password: "Password!123",
            DefaultShippingAddress: null
        );

        using var scope = _serviceProvider.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var result = await sender.Send(command);
        result.IsSuccess.Should().BeTrue();

        var identityUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        identityUser.Should().NotBeNull("The user must be created in the AspNetUsers table");

        var userId = identityUser!.Id;

        var userProfile = await dbContext.UserProfiles.FirstOrDefaultAsync(p => p.Id == userId);
        userProfile.Should().NotBeNull("The user must be creaeted in the UserProfile table");

        userProfile!.Name.Should().Be("John");
        userProfile.LastName.Should().Be("Doe");
    }
}