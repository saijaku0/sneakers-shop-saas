using SneakersShop.Application.Abstractions.Authentication;
using SneakersShop.Application.Abstractions.Commands;
using SneakersShop.Application.Abstractions.Repositories;
using SneakersShop.Application.Auth.DTOs;
using SneakersShop.Domain.Common.Results;

namespace SneakersShop.Application.Auth.Commands.LoginUser;

public sealed class LoginUserHandler(
    IAuthenticationService authService,
    IUnitOfWork unitOfWork)
    : CommandHandler<LoginCommand, AuthResponse>(unitOfWork)
{
    protected override async Task<Result<AuthResponse>> HandleCommandAsync(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        return await authService.LoginAsync(request.Email, request.Password, cancellationToken);
    }
}