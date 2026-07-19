using SneakersShop.Domain.Common.DomainEvent;

namespace SneakersShop.Domain.Orders.DomainEvents;

public sealed record OrderPaid(Guid OrderId) : DomainEventBase;