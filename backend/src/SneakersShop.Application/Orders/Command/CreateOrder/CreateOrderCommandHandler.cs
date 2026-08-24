using Microsoft.EntityFrameworkCore;

using SneakersShop.Application.Abstractions.ApplicatonDbContext;
using SneakersShop.Application.Abstractions.Authentication;
using SneakersShop.Application.Abstractions.Commands;
using SneakersShop.Application.Abstractions.Repositories;
using SneakersShop.Domain.Carts.Errors;
using SneakersShop.Domain.Common.Results;
using SneakersShop.Domain.Common.ValueObjects;
using SneakersShop.Domain.Consumer.Errors;
using SneakersShop.Domain.Orders;
using SneakersShop.Domain.Orders.Errors;
using SneakersShop.Domain.Warehouse.Errors;

namespace SneakersShop.Application.Orders.Command.CreateOrder;

internal sealed class CreateOrderCommandHandler(
    ICartRepository cartRepository,
    IWarehouseItemRepository warehouseRepository,
    IOrderRepository orderRepository,
    IApplicationDbContext context,
    ICurrentUserService userService,
    TimeProvider timeProvider,
    IUnitOfWork unitOfWork) : CommandHandler<CreateOrderCommand, Guid>(unitOfWork)
{
    protected override async Task<Result<Guid>> HandleCommandAsync(
        CreateOrderCommand command,
        CancellationToken cancellationToken)
    {
        var userId = userService.GetUserId();
        if (userId is null)
            return Result<Guid>.Failure(UserErrors.UserIsUnauthorized);

        var cart = await cartRepository.GetByUserIdAsync(userId.Value, cancellationToken);
        if (cart is null || cart.Items.Count == 0)
            return Result<Guid>.Failure(CartErrors.CartNotFound);

        var address = command.ShippingAddress;
        if (address is null)
        {
            var profile = await context.UserProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId.Value, cancellationToken);
            address = profile?.DefaultAddress;
        }

        if (address is null)
            return Result<Guid>.Failure(OrderError.ShippingAddressRequired);

        var warehouseItemIds = cart.Items.Select(i => i.WarehouseItemId).ToList();
        var prices = await context.WarehouseItems
            .AsNoTracking()
            .Where(w => warehouseItemIds.Contains(w.Id))
            .Select(w => new
            {
                WarehouseItemId = w.Id,
                Price = context.ProductVariants
                    .Where(v => v.Id == w.ProductVariantId)
                    .Select(v => context.Products
                        .Where(p => p.Id == v.ProductId)
                        .Select(p => p.BasePrice)
                        .First())
                    .First()
            })
            .ToDictionaryAsync(x => x.WarehouseItemId, x => x.Price, cancellationToken);

        var warehouseItems = await warehouseRepository
            .GetByIdsAsync(warehouseItemIds, cancellationToken);
        var warehouseMap = warehouseItems.ToDictionary(w => w.Id);

        var now = timeProvider.GetUtcNow();
        var order = Order.Create(userId.Value, address, command.PaymentMethod, now);

        foreach (var cartItem in cart.Items)
        {
            if (!warehouseMap.TryGetValue(cartItem.WarehouseItemId, out var warehouse))
                return Result<Guid>.Failure(WarehouseError.ItemNotFound(cartItem.WarehouseItemId));

            var reserve = warehouse.Reserve(cartItem.Quantity);
            if (reserve.IsFailure)
                return Result<Guid>.Failure(reserve.Error!);

            if (!prices.TryGetValue(cartItem.WarehouseItemId, out var unitPrice))
                return Result<Guid>.Failure(WarehouseError.ItemNotFound(cartItem.WarehouseItemId));

            var addResult = order.AddItem(cartItem.WarehouseItemId, cartItem.Quantity, unitPrice, 0m);
            if (addResult.IsFailure)
                return Result<Guid>.Failure(addResult.Error!);
        }

        await orderRepository.AddAsync(order, cancellationToken);

        // Mock paymend Pending -> Paid
        var pay = order.Pay(now);
        if (pay.IsFailure)
            return Result<Guid>.Failure(pay.Error!);

        cart.Clear();

        return Result<Guid>.Success(order.Id);
    }
}