using SneakersShop.Domain.Common.DomainEvent;

namespace SneakersShop.Domain.Warehouse.DomainEvents;

public sealed record WarehouseItemReleased(Guid WarehouseItemId, int ReleasedQuantity) : DomainEventBase;