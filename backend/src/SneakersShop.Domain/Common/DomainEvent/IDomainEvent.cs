namespace SneakersShop.Domain.Common.DomainEvent;

public interface IDomainEvent
{
    DateTimeOffset OccurredAt { get; }
}