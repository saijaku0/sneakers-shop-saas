using SneakersShop.Domain.Common.DomainEvent;

namespace SneakersShop.Domain.Catalog.DomainEvents;

public sealed record ProductDeactivated(Guid ProductId) : DomainEventBase;
