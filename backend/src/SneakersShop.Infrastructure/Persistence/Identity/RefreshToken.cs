using SneakersShop.Domain.Common.Guards;
using SneakersShop.Domain.Common.Results;

namespace SneakersShop.Infrastructure.Persistence.Identity;

/// <summary>
/// Represents a refresh token used for authentication and authorization purposes.
/// </summary>
/// <remarks>
/// A refresh token is a credential that is used to obtain a new access token without requiring the user to re-authenticate. 
/// It is typically issued alongside an access token and has a longer expiration time. 
/// Refresh tokens are commonly used in scenarios where access tokens have short lifespans, 
/// allowing users to maintain their authenticated sessions without frequent logins.
/// </remarks>
public sealed class RefreshToken
{
    public Guid TokenId { get; private init; }
    public string Token { get; private set; } = null!;
    public Guid UserId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }
    public bool IsRevoked { get; private set; }

    private RefreshToken() { } // Required by EF Core
    private RefreshToken(
        string token,
        Guid userId,
        DateTimeOffset createdAt,
        DateTimeOffset expiresAt)
    {
        TokenId = Guid.NewGuid();
        Token = token;
        UserId = userId;
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
        IsRevoked = false;
    }

    public static RefreshToken Create(
        string token,
        Guid userId,
        DateTimeOffset createdAt,
        DateTimeOffset expiresAt)
    {
        Guard.Against.NullOrEmpty(token);
        Guard.Against.Empty(userId);
        Guard.Against.Default(createdAt);
        Guard.Against.Default(expiresAt);

        return new RefreshToken(token, userId, createdAt, expiresAt);
    }

    public void Revoke()
    {
        if (IsRevoked)
            return;

        IsRevoked = true;
    }

    // Use TimeProvider to get the current time for testing purposes
    public bool IsExpired(DateTimeOffset now) => now >= ExpiresAt;
}