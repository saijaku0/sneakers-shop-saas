using SneakersShop.Domain.Common.Entities;
using SneakersShop.Domain.Common.Exceptions;
using SneakersShop.Domain.Common.Guards;

namespace SneakersShop.Domain.Orders;

public sealed class OrderItem : EntityBase
{
    public Guid WarehouseItemId { get; private init; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal TotalPrice => (UnitPrice * Quantity) - DiscountAmount;

    private OrderItem() { }
    private OrderItem(
        Guid id,
        Guid warehouseItemId,
        int quantity,
        decimal unitPrice,
        decimal discountAmount) : base(id)
    {
        WarehouseItemId = warehouseItemId;
        Quantity = quantity;
        UnitPrice = unitPrice;
        DiscountAmount = discountAmount;
    }

    internal static OrderItem Create(
        Guid warehouseItemId,
        int quantity,
        decimal unitPrice,
        decimal discountAmount)
    {
        Guard.Against.Empty(warehouseItemId);
        Guard.Against.NegativeOrZero(quantity);
        Guard.Against.NegativeOrZero(unitPrice);
        Guard.Against.Negative(discountAmount);

        if (discountAmount > unitPrice * quantity)
            throw new DomainException(
                $"Discount {discountAmount} exceeds line total {unitPrice * quantity}.",
                nameof(discountAmount));

        return new OrderItem(
            Guid.CreateVersion7(),
            warehouseItemId,
            quantity,
            unitPrice,
            discountAmount);
    }
}