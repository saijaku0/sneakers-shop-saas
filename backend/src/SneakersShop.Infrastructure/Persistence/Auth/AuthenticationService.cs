using Microsoft.AspNetCore.Identity;

using SneakersShop.Application.Abstractions.Authentication;
using SneakersShop.Application.Abstractions.Repositories;
using SneakersShop.Application.Auth.Commands.RegisterNewUser;
using SneakersShop.Application.Auth.DTOs;
using SneakersShop.Domain.Common.Results;
using SneakersShop.Domain.Consumer;
using SneakersShop.Domain.Consumer.Enums;
using SneakersShop.Infrastructure.Persistence.Identity;

namespace SneakersShop.Infrastructure.Persistence.Auth;

internal class AuthenticationService(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole<Guid>> roleManager,
    IUnitOfWork unitOfWork,
    AppDbContext appDbContext)
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
                return Result<Guid>.Failure(Error.Conflict("Auth.RegistrationFailed", "Failed to create user."));

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
            return Result<bool>.Failure(Error.NotFound("Auth.UserNotFound", $"User with id '{userId}' not found."));
        return Result<bool>.Success(await userManager.CheckPasswordAsync(user, password));
    }
    public async Task<Result> AssignRoleAsync(Guid userId, UserRoles role, CancellationToken cancellation = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return Result.Failure(Error.NotFound("Auth.UserNotFound", $"User with id '{userId}' not found."));

        var roleName = role.ToString();
        var roleExists = await roleManager.RoleExistsAsync(roleName);
        if (!roleExists)
            return Result.Failure(Error.NotFound("Auth.RoleNotFound", $"Role '{roleName}' not found."));

        var addToRoleResult = await userManager.AddToRoleAsync(user, role.ToString());
        if (!addToRoleResult.Succeeded)
            return Result.Failure(Error.Conflict("Auth.RoleAssignmentFailed", "Failed to assign role to user."));

        return Result.Success();
    }
    public async Task<Result<Guid?>> FindUserByEmailAsync(string email, CancellationToken cancellation = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return Result<Guid?>.Failure(Error.NotFound("Auth.UserNotFound", $"User with email '{email}' not found."));
        return Result<Guid?>.Success(user.Id);
    }
    public async Task<Result<TokenGenerationRequest>> GetTokenGenerationDataAsync(Guid userId, CancellationToken cancellation = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return Result<TokenGenerationRequest>.Failure(Error.NotFound("Auth.UserNotFound", $"User with id '{userId}' not found."));
        var userRoles = await userManager.GetRolesAsync(user);

        var roles = userRoles.Select(r => Enum.Parse<UserRoles>(r)).ToList();
        return Result<TokenGenerationRequest>.Success(new TokenGenerationRequest(
            user.Id,
            user.Email ?? string.Empty,
            roles));
    }
}