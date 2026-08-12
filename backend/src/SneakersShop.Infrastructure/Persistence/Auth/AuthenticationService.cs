using Microsoft.AspNetCore.Identity;

using SneakersShop.Application.Abstractions.Authentication;
using SneakersShop.Application.Abstractions.Repositories;
using SneakersShop.Application.Auth.Commands.RegisterNewUser;
using SneakersShop.Application.Auth.DTOs;
using SneakersShop.Domain.Common.Results;
using SneakersShop.Domain.Consumer;
using SneakersShop.Domain.Consumer.Enums;
using SneakersShop.Infrastructure.Persistence.Auth.Abstractions;
using SneakersShop.Infrastructure.Persistence.Auth.Errors;
using SneakersShop.Infrastructure.Persistence.Identity;

namespace SneakersShop.Infrastructure.Persistence.Auth;

internal sealed class AuthenticationService(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole<Guid>> roleManager,
    IRefreshTokenRepository refreshTokenRepository,
    IJwtService jwtService,
    IUnitOfWork unitOfWork,
    AppDbContext appDbContext,
    TimeProvider timeProvider)
    : IAuthenticationService
{
    public async Task<Result<Guid>> CreateUserAsync(RegisterCommand req, CancellationToken cancellation = default)
    {
        await using var transaction = await appDbContext.Database.BeginTransactionAsync(cancellation);
        try
        {
            var userApplication = new ApplicationUser
            {
                UserName = req.Email,
                Email = req.Email,
                PhoneNumber = req.PhoneNumber
            };

            var createUserResult = await userManager.CreateAsync(userApplication, req.Password);
            if (!createUserResult.Succeeded)
                return Result<Guid>.Failure(AuthError.RegistrationFailed);

            var userDomain = UserProfile.Create(
                userApplication.Id,
                req.DefaultShippingAddress,
                req.Name,
                req.LastName);

            await appDbContext.UserProfiles.AddAsync(userDomain, cancellation);
            await unitOfWork.SaveChangesAsync(cancellation);
            await transaction.CommitAsync(cancellation);
            return Result<Guid>.Success(userApplication.Id);
        }
        catch
        {
            await transaction.RollbackAsync(cancellation);
            throw;
        }
    }
    public async Task<Result<bool>> CheckUserPasswordAsync(Guid userId, string password, CancellationToken cancellation = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return Result<bool>.Failure(AuthError.UserNotFound);
        return Result<bool>.Success(await userManager.CheckPasswordAsync(user, password));
    }
    public async Task<Result> AssignRoleAsync(Guid userId, UserRoles role, CancellationToken cancellation = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return Result.Failure(AuthError.UserNotFound);

        var roleName = role.ToString();
        var roleExists = await roleManager.RoleExistsAsync(roleName);
        if (!roleExists)
            return Result.Failure(AuthError.RoleNotFound(roleName));

        var addToRoleResult = await userManager.AddToRoleAsync(user, role.ToString());
        if (!addToRoleResult.Succeeded)
            return Result.Failure(AuthError.RoleAssignmentFailed);

        return Result.Success();
    }
    public async Task<Result<Guid?>> FindUserByEmailAsync(string email, CancellationToken cancellation = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return Result<Guid?>.Failure(AuthError.UserNotFound);
        return Result<Guid?>.Success(user.Id);
    }
    public async Task<Result<TokenGenerationRequest>> GetTokenGenerationDataAsync(Guid userId, CancellationToken cancellation = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return Result<TokenGenerationRequest>.Failure(AuthError.UserNotFound);
        var userRoles = await userManager.GetRolesAsync(user);

        var roles = userRoles.Select(r => Enum.Parse<UserRoles>(r)).ToList();
        return Result<TokenGenerationRequest>.Success(new TokenGenerationRequest(
            user.Id,
            user.Email ?? string.Empty,
            roles));
    }

    public async Task<Result<AuthResponse>> LoginAsync(string email, string password, CancellationToken ct = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return Result<AuthResponse>.Failure(AuthError.InvalidCredentials);

        if (!await userManager.CheckPasswordAsync(user, password))
            return Result<AuthResponse>.Failure(AuthError.InvalidCredentials);

        var tokenGenerationData = await GetTokenGenerationDataAsync(user.Id, ct);
        if (!tokenGenerationData.IsSuccess)
            return Result<AuthResponse>.Failure(tokenGenerationData.Error);

        var accessToken = jwtService.GenerateAccessToken(tokenGenerationData.Value);
        var refreshToken = jwtService.GenerateRefreshToken();

        var now = timeProvider.GetUtcNow();
        var expiresAt = now.AddDays(jwtService.GetRefreshTokenExpiryDays());

        var refreshEntity = RefreshToken.Create(refreshToken, user.Id, now, expiresAt);
        await refreshTokenRepository.AddAsync(refreshEntity, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result<AuthResponse>.Success(new AuthResponse(accessToken, refreshToken));
    }

    public async Task<Result<AuthResponse>> RefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        var refreshEntity = await refreshTokenRepository.GetByTokenAsync(refreshToken, ct);
        if (refreshEntity is null)
            return Result<AuthResponse>.Failure(AuthError.InvalidRefreshToken);

        var now = timeProvider.GetUtcNow();

        if (refreshEntity.IsRevoked)
        {
            await refreshTokenRepository.RevokeAllByUserIdAsync(refreshEntity.UserId, ct);
            await unitOfWork.SaveChangesAsync(ct);

            return Result<AuthResponse>.Failure(AuthError.BreachDetected);
        }

        if (refreshEntity.IsExpired(now))
            return Result<AuthResponse>.Failure(AuthError.ExpiredRefreshToken);

        var user = await userManager.FindByIdAsync(refreshEntity.UserId.ToString());
        if (user is null)
            return Result<AuthResponse>.Failure(AuthError.UserNotFound);

        var tokenGenerationData = await GetTokenGenerationDataAsync(user.Id, ct);
        if (!tokenGenerationData.IsSuccess)
            return Result<AuthResponse>.Failure(tokenGenerationData.Error);

        var newAccessToken = jwtService.GenerateAccessToken(tokenGenerationData.Value);
        var newRefreshToken = jwtService.GenerateRefreshToken();
        var expiresAt = now.AddDays(jwtService.GetRefreshTokenExpiryDays());

        refreshEntity.Revoke();

        var newRefreshEntity = RefreshToken.Create(
            token: newRefreshToken,
            userId: user.Id,
            createdAt: now,
            expiresAt: expiresAt
        );

        await refreshTokenRepository.AddAsync(newRefreshEntity, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result<AuthResponse>.Success(new AuthResponse(newAccessToken, newRefreshToken));
    }

    public async Task<Result> LogoutAsync(string refreshToken, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return Result.Failure(AuthError.TokenRequired);

        var refreshEntity = await refreshTokenRepository.GetByTokenAsync(refreshToken, ct);

        if (refreshEntity is null)
            return Result.Success();
        if (refreshEntity.IsRevoked)
            return Result.Success();

        refreshEntity.Revoke();

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}