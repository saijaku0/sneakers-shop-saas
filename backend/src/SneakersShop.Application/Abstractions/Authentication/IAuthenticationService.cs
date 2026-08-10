using SneakersShop.Application.Auth.Commands.RegisterNewUser;
using SneakersShop.Application.Auth.DTOs;
using SneakersShop.Domain.Common.Results;
using SneakersShop.Domain.Consumer.Enums;

namespace SneakersShop.Application.Abstractions.Authentication;

public interface IAuthenticationService
{
    Task<Result<Guid>> CreateUserAsync(RegisterCommand req, CancellationToken cancellation = default);
    Task<Result<bool>> CheckUserPasswordAsync(Guid userId, string password, CancellationToken cancellation = default);
    Task<Result> AssignRoleAsync(Guid userId, UserRoles role, CancellationToken cancellation = default);
    Task<Result<Guid?>> FindUserByEmailAsync(string email, CancellationToken cancellation = default);
    Task<Result<TokenGenerationRequest>> GetTokenGenerationDataAsync(Guid userId, CancellationToken cancellation = default);
}