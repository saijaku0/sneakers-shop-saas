using SneakersShop.Domain.Common.Entities;
using SneakersShop.Domain.Common.Guards;

namespace SneakersShop.Domain.Carts;

public sealed class Cart : AggregateRoot
{
    private readonly List<CartItem> _items = [];
    public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();
    public Guid UserId { get; private init; }

    private Cart() { }
    private Cart(
        Guid id,
        Guid userId) : base(id)
    {
        UserId = userId;
    }

    public static Cart Create(Guid userId)
    {
        Guard.Against.Empty(userId);

        return new Cart(Guid.CreateVersion7(), userId);
    }

    public void AddItem(Guid warehouseItemId, int quantity)
    {
        Guard.Against.Empty(warehouseItemId);
        Guard.Against.NegativeOrZero(quantity);

        var existing = _items.SingleOrDefault(i => i.WarehouseItemId == warehouseItemId);
        if (existing is not null)
            existing.IncreaseQuantity(quantity);
        else
            _items.Add(CartItem.Create(warehouseItemId, quantity));

        Touch();
    }

    public void ChangeQuantity(Guid warehouseItemId, int quantity)
    {
        Guard.Against.Empty(warehouseItemId);

        var item = _items.SingleOrDefault(i => i.WarehouseItemId == warehouseItemId);
        if (item is null)
            return;

        if (quantity <= 0)
            _items.Remove(item);
        else
            item.SetQuantity(quantity);

        Touch();
    }

    public void RemoveItem(Guid warehouseItemId)
    {
        Guard.Against.Empty(warehouseItemId);

        var item = _items.SingleOrDefault(i => i.WarehouseItemId == warehouseItemId);
        if (item is null)
            return;

        _items.Remove(item);
        Touch();
    }

    public void Clear()
    {
        if (_items.Count == 0)
            return;

        _items.Clear();
        Touch();
    }
}