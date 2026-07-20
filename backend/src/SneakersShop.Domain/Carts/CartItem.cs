using SneakersShop.Domain.Common.Entities;
using SneakersShop.Domain.Common.Guards;

namespace SneakersShop.Domain.Carts;

public sealed class CartItem : EntityBase
{
    public Guid WarehouseItemId { get; private init; }
    public int Quantity { get; private set; }

    private CartItem() { }

    private CartItem(
        Guid id,
        Guid warehouseItemId,
        int quantity) : base(id)
    {
        WarehouseItemId = warehouseItemId;
        Quantity = quantity;
    }

    internal static CartItem Create(
        Guid warehouseItemId,
        int quantity)
    {
        Guard.Against.Empty(warehouseItemId);
        Guard.Against.NegativeOrZero(quantity);

        return new CartItem(Guid.CreateVersion7(), warehouseItemId, quantity);
    }

    internal void IncreaseQuantity(int by)
    {
        Guard.Against.NegativeOrZero(by);
        Quantity += by;
    }

    internal void SetQuantity(int value)
    {
        Guard.Against.NegativeOrZero(value);
        Quantity = value;
    }
}