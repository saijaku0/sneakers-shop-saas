using SneakersShop.Application.Abstractions.Authentication;
using SneakersShop.Application.Abstractions.Commands;
using SneakersShop.Application.Abstractions.Repositories;
using SneakersShop.Application.Auth.DTOs;
using SneakersShop.Domain.Common.Results;

namespace SneakersShop.Application.Auth.Commands.RegisterNewUser;

public sealed class RegisterNewUserHandler(
    IAuthenticationService authService,
    IUnitOfWork unitOfWork)
    : CommandHandler<RegisterCommand, AuthResponse>(unitOfWork)
{
    protected override async Task<Result<AuthResponse>> HandleCommandAsync(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        var createUser = await authService.CreateUserAsync(request, cancellationToken).ConfigureAwait(false);
        if (createUser == null)
            return Result<AuthResponse>.Failure(new Error("Auth.InvalidCredentials", "User creation failed.", ErrorType.Invalid));

        return await authService.LoginAsync(request.Email, request.Password, cancellationToken);
    }
}