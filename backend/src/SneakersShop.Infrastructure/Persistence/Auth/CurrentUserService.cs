using System.Security.Claims;

using Microsoft.AspNetCore.Http;

using SneakersShop.Application.Abstractions.Authentication;

namespace SneakersShop.Infrastructure.Persistence.Auth;

internal sealed class CurrentUserService(IHttpContextAccessor httpContext) : ICurrentUserService
{
    public Guid? GetUserId()
    {
        var user = httpContext.HttpContext?.User;
        var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier)
                            ?? user?.FindFirst("sub");
        if (userIdClaim is null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return null;
        return userId;
    }
}