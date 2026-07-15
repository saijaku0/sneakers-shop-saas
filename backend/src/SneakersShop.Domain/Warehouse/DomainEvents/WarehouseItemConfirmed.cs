
using SneakersShop.Domain.Common.DomainEvent;

namespace SneakersShop.Domain.Warehouse.DomainEvents;
public sealed record WarehouseItemConfirmed(Guid WarehouseItemId, int ConfirmedQuantity) : DomainEventBase;
