using SneakersShop.Domain.Consumer.Enums;

namespace SneakersShop.Application.Auth.DTOs;

public record TokenGenerationRequest(
    Guid UserId,
    string Email,
    IEnumerable<UserRoles> Roles
);