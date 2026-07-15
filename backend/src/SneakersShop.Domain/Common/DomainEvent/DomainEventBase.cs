namespace SneakersShop.Domain.Common.DomainEvent;

public abstract record DomainEventBase : IDomainEvent
{
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}
