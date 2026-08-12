using System.Security.Claims;

using SneakersShop.Application.Auth.DTOs;

namespace SneakersShop.Infrastructure.Persistence.Auth.Abstractions;

public interface IJwtService
{
    string GenerateAccessToken(TokenGenerationRequest request);
    string GenerateRefreshToken();
    int GetRefreshTokenExpiryDays();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}