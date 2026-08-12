using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using SneakersShop.Application.Auth.DTOs;
using SneakersShop.Infrastructure.Persistence.Auth.Abstractions;

namespace SneakersShop.Infrastructure.Persistence.Auth;

internal sealed class JwtService(IOptions<JwtSettings> jwtOptions)
    : IJwtService
{
    public string GenerateAccessToken(TokenGenerationRequest request)
    {
        var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, request.UserId.ToString()),
                new(ClaimTypes.Email, request.Email ?? string.Empty),
            };

        foreach (var roles in request.Roles)
            claims.Add(new Claim(ClaimTypes.Role, roles.ToString()));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Value.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtOptions.Value.Issuer,
            audience: jwtOptions.Value.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(jwtOptions.Value.AccessTokenExpiryMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var token = RandomNumberGenerator.GetBytes(64);
        var newToken = Convert.ToBase64String(token);
        return newToken;
    }

    public int GetRefreshTokenExpiryDays() => jwtOptions.Value.RefreshTokenExpiryDays;

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var parameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtOptions.Value.SecretKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Value.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Value.Audience,
            ValidateLifetime = false
        };

        var handler = new JwtSecurityTokenHandler();
        var principal = handler.ValidateToken(token, parameters, out SecurityToken securityToken);

        if (securityToken is not JwtSecurityToken jwtToken ||
            !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }
}