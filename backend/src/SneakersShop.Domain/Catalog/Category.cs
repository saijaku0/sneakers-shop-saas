using SneakersShop.Domain.Common.Entities;
using SneakersShop.Domain.Common.Guards;

namespace SneakersShop.Domain.Catalog;

public sealed class Category : AggregateRoot
{
    public string Name { get; private set; } = null!;

    private Category() { }

    private Category(Guid id, string name) : base(id)
    {
        Name = name;
    }

    public static Category Create(string name)
    {
        Guard.Against.NullOrWhiteSpace(name);
        return new Category(Guid.CreateVersion7(), name);
    }

    public void UpdateName(string name)
    {
        Guard.Against.NullOrWhiteSpace(name);
        Name = name;
        Touch();
    }
}