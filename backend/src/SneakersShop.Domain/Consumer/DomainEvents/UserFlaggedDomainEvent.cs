using SneakersShop.Domain.Common.DomainEvent;

namespace SneakersShop.Domain.Consumer.DomainEvents;

public record UserFlaggedDomainEvent(Guid UserId) : DomainEventBase;