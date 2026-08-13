using FluentAssertions;

using SneakersShop.Infrastructure.Persistence.Identity;

namespace SneakersShop.Domain.UnitTests.TokenRefresh;

public class RefreshTokenTests
{
    private readonly Guid _validUserId = Guid.NewGuid();
    private readonly DateTimeOffset _validCreatedAt = DateTimeOffset.UtcNow;
    private readonly DateTimeOffset _validExpiresAt = DateTimeOffset.UtcNow.AddDays(7);

    [Fact]
    public void Create_WithNullToken_ThrowsException()
    {
        Action act = () => RefreshToken.Create(null!, _validUserId, _validCreatedAt, _validExpiresAt);

        act.Should().Throw<ArgumentException>("The token cannot be null");
    }

    [Fact]
    public void Create_WithEmptyToken_ThrowsException()
    {
        Action act = () => RefreshToken.Create(string.Empty, _validUserId, _validCreatedAt, _validExpiresAt);

        act.Should().Throw<ArgumentException>("The token cannot be empty");
    }

    [Fact]
    public void Create_WithEmptyUserId_ThrowsException()
    {
        Action act = () => RefreshToken.Create("valid-token", Guid.Empty, _validCreatedAt, _validExpiresAt);

        act.Should().Throw<ArgumentException>("UserId cannot be an empty Guid");
    }

    [Fact]
    public void Create_WithDefaultCreatedAt_ThrowsException()
    {
        Action act = () => RefreshToken.Create("valid-token", _validUserId, default, _validExpiresAt);

        act.Should().Throw<ArgumentException>("CreatedAt cannot be default");
    }

    [Fact]
    public void Create_WithDefaultExpiresAt_ThrowsException()
    {
        Action act = () => RefreshToken.Create("valid-token", _validUserId, _validCreatedAt, default);

        act.Should().Throw<ArgumentException>("ExpiresAt cannot be default");
    }

    [Fact]
    public void Revoke_Twice_ShouldBeIdempotent()
    {
        var token = RefreshToken.Create("valid-token", _validUserId, _validCreatedAt, _validExpiresAt);

        token.Revoke();

        token.Revoke();

        token.IsRevoked.Should().BeTrue(
            "calling Revoke multiple times should not change the state or throw an exception");
    }

    [Fact]
    public void IsExpired_WhenNowIsBeforeExpiration_ReturnsFalse()
    {
        var token = RefreshToken.Create("valid-token", _validUserId, _validCreatedAt, _validExpiresAt);

        var timeBeforeExpiration = _validExpiresAt.AddDays(-1);

        token.IsExpired(timeBeforeExpiration).Should().BeFalse();
    }

    [Fact]
    public void IsExpired_WhenNowIsExactlyAtExpiration_ReturnsTrue()
    {
        var token = RefreshToken.Create("valid-token", _validUserId, _validCreatedAt, _validExpiresAt);

        token.IsExpired(_validExpiresAt).Should().BeTrue(
            "the token is considered expired exactly at ExpiresAt");
    }

    [Fact]
    public void IsExpired_WhenNowIsAfterExpiration_ReturnsTrue()
    {
        var token = RefreshToken.Create("valid-token", _validUserId, _validCreatedAt, _validExpiresAt);

        var timeAfterExpiration = _validExpiresAt.AddMinutes(1);

        token.IsExpired(timeAfterExpiration).Should().BeTrue();
    }
}