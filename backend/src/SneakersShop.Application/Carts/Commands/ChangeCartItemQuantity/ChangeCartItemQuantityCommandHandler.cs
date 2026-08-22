using SneakersShop.Application.Abstractions.Authentication;
using SneakersShop.Application.Abstractions.Commands;
using SneakersShop.Application.Abstractions.Repositories;
using SneakersShop.Domain.Carts.Errors;
using SneakersShop.Domain.Common.Results;
using SneakersShop.Domain.Consumer.Errors;

namespace SneakersShop.Application.Carts.Commands.ChangeCartItemQuantity;

internal sealed class ChangeCartItemQuantityCommandHandler(
    ICartRepository cartRepository,
    ICurrentUserService userService,
    IUnitOfWork unitOfWork) : CommandHandler<ChangeCartItemQuantityCommand>(unitOfWork)
{
    protected override async Task<Result> HandleCommandAsync(
        ChangeCartItemQuantityCommand command,
        CancellationToken cancellationToken)
    {
        var userId = userService.GetUserId();
        if (userId is null)
            return Result.Failure(UserErrors.UserIsUnauthorized);

        var cart = await cartRepository.GetByUserIdAsync(userId.Value, cancellationToken);
        if (cart is null)
            return Result.Failure(CartErrors.CartNotFound);

        cart.ChangeQuantity(command.WarehouseItemId, command.Quantity);
        return Result.Success();
    }
}