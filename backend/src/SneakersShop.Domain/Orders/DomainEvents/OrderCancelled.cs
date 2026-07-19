using SneakersShop.Domain.Common.DomainEvent;

namespace SneakersShop.Domain.Orders.DomainEvents;

public sealed record OrderCancelled(Guid OrderId, DateTimeOffset CancelledAt) : DomainEventBase;