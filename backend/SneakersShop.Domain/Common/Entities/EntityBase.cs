namespace SneakersShop.Domain.Common.Entities;

public abstract class EntityBase : Entity<Guid>
{
    public DateTimeOffset CreatedAt { get; protected init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; protected set; }
    protected EntityBase(Guid id) : base(id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id cannot be empty.", nameof(id));
    }
    protected EntityBase() { }
    protected void Touch() => UpdatedAt = DateTimeOffset.UtcNow;
}
