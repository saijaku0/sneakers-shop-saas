using SneakersShop.Domain.Common.Entities;
using SneakersShop.Domain.Common.Guards;
using SneakersShop.Domain.Common.Results;
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
}

