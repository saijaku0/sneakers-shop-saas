using SneakersShop.Domain.Common.DomainEvent;

namespace SneakersShop.Domain.Warehouse.DomainEvents;

public sealed record WarehouseItemReserved(Guid WarehouseItemId, int ReservedQuantity) : DomainEventBase;