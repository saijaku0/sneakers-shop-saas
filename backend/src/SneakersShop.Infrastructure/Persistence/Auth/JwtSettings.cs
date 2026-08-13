namespace SneakersShop.Infrastructure.Persistence.Auth;

/// <summary>
/// Represents the settings for JWT (JSON Web Token) authentication.
/// </summary>
public sealed class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int AccessTokenExpiryMinutes { get; set; }
    public int RefreshTokenExpiryDays { get; set; }
}