using SneakersShop.Application.Abstractions.Authentication;
using SneakersShop.Application.Abstractions.Commands;
using SneakersShop.Application.Abstractions.Repositories;
using SneakersShop.Domain.Carts;
using SneakersShop.Domain.Common.Results;
using SneakersShop.Domain.Consumer.Errors;
using SneakersShop.Domain.Warehouse.Errors;

namespace SneakersShop.Application.Carts.Commands.AddCartItem;

internal sealed class AddCartItemCommandHandler(
    ICartRepository cartRepository,
    ICurrentUserService userService,
    IWarehouseItemRepository itemRepository,
    IUnitOfWork unitOfWork) : CommandHandler<AddCartItemCommand>(unitOfWork)
{
    protected override async Task<Result> HandleCommandAsync(
        AddCartItemCommand command,
        CancellationToken cancellationToken)
    {
        var userId = userService.GetUserId();
        if (userId is null)
            return Result.Failure(UserErrors.UserIsUnauthorized);

        var warehouseItem = await itemRepository.GetByIdAsync(command.WarehouseItemId, cancellationToken);
        if (warehouseItem is null)
            return Result.Failure(WarehouseError.ItemNotFound(command.WarehouseItemId));
        if (warehouseItem.Available < command.Quantity)
            return Result.Failure(WarehouseError.InsufficientStock(command.Quantity, warehouseItem.Available));

        var cart = await cartRepository.GetByUserIdAsync(userId.Value, cancellationToken);
        if (cart is null)
        {
            cart = Cart.Create(userId.Value);
            await cartRepository.AddAsync(cart, cancellationToken);
        }
        cart.AddItem(command.WarehouseItemId, command.Quantity);
        return Result.Success();
    }
}