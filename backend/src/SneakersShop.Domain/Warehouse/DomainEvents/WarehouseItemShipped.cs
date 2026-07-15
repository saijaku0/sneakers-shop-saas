
using SneakersShop.Domain.Common.DomainEvent;

namespace SneakersShop.Domain.Warehouse.DomainEvents;

public sealed record WarehouseItemShipped(Guid WarehouseItemId, int ConfirmedQuantity) : DomainEventBase;