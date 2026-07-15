using SneakersShop.Domain.Common.Entities;
using SneakersShop.Domain.Common.Exceptions;
using SneakersShop.Domain.Common.Guards;
using SneakersShop.Domain.Common.Results;
using SneakersShop.Domain.Warehouse.DomainEvents;
using SneakersShop.Domain.Warehouse.Errors;
using SneakersShop.Domain.Warehouse.ValueObjects;

using static SneakersShop.Domain.Warehouse.WarehouseItemPolicy;

namespace SneakersShop.Domain.Warehouse;

public sealed class WarehouseItem : AggregateRoot
{
    public Guid ProductId { get; private init; }
    public Size Size { get; private init; } = null!;
    public int Quantity { get; private set; }
    public int ReservedQuantity { get; private set; }
    public byte[]? RowVersion { get; private set; }

    public int Available => Quantity - ReservedQuantity;

    private WarehouseItem() { }
    private WarehouseItem(
        Guid productId,
        Size size,
        int quantity) : base(Guid.CreateVersion7())
    {
        ProductId = productId;
        Size = size;
        Quantity = quantity;
        ReservedQuantity = 0;
    }

    public static Result<WarehouseItem> Create(
        Guid productId,
        Size size,
        int quantity)
    {
        Guard.Against.Empty(productId);
        Guard.Against.Null(size);
        Guard.Against.NegativeOrZero(quantity);

        if (quantity > MaximumQuantity)
            return WarehouseError.QuantityExceedsMaximum(quantity, MaximumQuantity);

        return new WarehouseItem(productId, size, quantity);
    }

    public Result Reserve(int quantity)
    {
        Guard.Against.NegativeOrZero(quantity);
        if (quantity > Available)
            return WarehouseError.InsufficientStock(quantity, Available);
        ReservedQuantity += quantity;
        EnsureInvariant();
        AddDomainEvent(new WarehouseItemReserved(Id, quantity));
        Touch();
        return Result.Success();
    }

    public Result Release(int quantity)
    {
        Guard.Against.NegativeOrZero(quantity);
        if (quantity > ReservedQuantity)
            throw new DomainException(
                $"Cannot release {quantity}: only {ReservedQuantity} reserved.",
                nameof(quantity));
        ReservedQuantity -= quantity;
        EnsureInvariant();
        AddDomainEvent(new WarehouseItemReleased(Id, quantity));
        Touch();
        return Result.Success();
    }

    public Result ConfirmShipment(int quantity)
    {
        Guard.Against.NegativeOrZero(quantity);
        if (quantity > ReservedQuantity)
            throw new DomainException(
                $"Cannot ship {quantity}: only {ReservedQuantity} reserved.",
                nameof(quantity));
        ReservedQuantity -= quantity;
        Quantity -= quantity;
        EnsureInvariant();
        AddDomainEvent(new WarehouseItemShipped(Id, quantity));
        Touch();
        return Result.Success();
    }

    private void EnsureInvariant()
    {
        if (ReservedQuantity < 0 || ReservedQuantity > Quantity)
            throw new DomainException(
                $"Invariant violated: reserved={ReservedQuantity}, quantity={Quantity}.");
    }
}