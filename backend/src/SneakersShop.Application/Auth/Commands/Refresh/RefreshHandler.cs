using SneakersShop.Application.Abstractions.Authentication;
using SneakersShop.Application.Abstractions.Commands;
using SneakersShop.Application.Abstractions.Repositories;
using SneakersShop.Application.Auth.DTOs;
using SneakersShop.Domain.Common.Results;

namespace SneakersShop.Application.Auth.Commands.Refresh;

public sealed class RefreshHandler(
    IAuthenticationService authService,
    IUnitOfWork unitOfWork)
    : CommandHandler<RefreshCommand, AuthResponse>(unitOfWork)
{
    protected override async Task<Result<AuthResponse>> HandleCommandAsync(
        RefreshCommand request,
        CancellationToken cancellationToken)
    {
        return await authService.RefreshTokenAsync(request.RefreshToken, cancellationToken);
    }
}