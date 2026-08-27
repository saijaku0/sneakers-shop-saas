using Microsoft.AspNetCore.Identity;

using SneakersShop.Domain.Common.Results;

namespace SneakersShop.Infrastructure.Persistence.Auth.Errors;

public static class AuthError
{
    public static Error RegistrationFailed =>
        Error.Conflict("auth.registration_failed", "Registration failed due to a conflict.");
    public static Error FromIdentity(IdentityError error) =>
        Error.Conflict($"auth.{error.Code.ToLower()}", error.Description);
    public static Error UserNotFound =>
        Error.NotFound("auth.user_not_found", "User not found.");
    public static Error RoleNotFound(string roleName) =>
        Error.NotFound("auth.role_not_found", $"Role '{roleName}' not found.");
    public static Error RoleAssignmentFailed =>
        Error.Conflict("auth.role_assignment_failed", "Failed to assign role to user.");
    public static Error InvalidCredentials =>
        Error.Unauthorized("auth.invalid_credentials", "Invalid email or password.");
    public static Error InvalidRefreshToken =>
        Error.Unauthorized("auth.invalid_refresh_token", "Invalid or expired refresh token.");
    public static Error BreachDetected =>
        Error.Unauthorized("auth.breach_detected", "Token reuse detected. All sessions revoked. Please login again.");
    public static Error ExpiredRefreshToken =>
        Error.Unauthorized("auth.expired_refresh_token", "Expired refresh token.");
    public static Error TokenRequired =>
        Error.Unauthorized("auth.token_required", "Token is required.");
}