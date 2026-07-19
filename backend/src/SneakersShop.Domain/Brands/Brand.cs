using SneakersShop.Domain.Common.Entities;
using SneakersShop.Domain.Common.Guards;

namespace SneakersShop.Domain.Brands;

public sealed class Brand : AggregateRoot
{
    public string Name { get; private set; }

    private Brand() { }

    private Brand(
        Guid id,
        string name) : base(id)
    {
        Name = name;
    }

    public static Brand Create(
        string name)
    {
        Guard.Against.NullOrWhiteSpace(name);

        return new Brand(Guid.CreateVersion7(), name);
    }

    public void UpdateName(
        string name)
    {
        if (name == Name)
            return;
        Guard.Against.NullOrWhiteSpace(name);
        Touch();
        Name = name;
    }
}