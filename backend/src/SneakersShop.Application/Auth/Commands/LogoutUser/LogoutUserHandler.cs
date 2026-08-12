using SneakersShop.Application.Abstractions.Authentication;
using SneakersShop.Application.Abstractions.Commands;
using SneakersShop.Application.Abstractions.Repositories;
using SneakersShop.Domain.Common.Results;

namespace SneakersShop.Application.Auth.Commands.LogoutUser;

public sealed class LogoutUserHandler(
    IAuthenticationService authService,
    IUnitOfWork unitOfWork)
    : CommandHandler<LogoutUserCommand>(unitOfWork)
{
    protected override async Task<Result> HandleCommandAsync(
        LogoutUserCommand request,
        CancellationToken cancellationToken)
    {
        return await authService.LogoutAsync(request.RefreshToken, cancellationToken);
    }
}