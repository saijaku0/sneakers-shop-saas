using System.Reflection;

using SneakersShop.Domain.Catalog.DomainEvents;
using SneakersShop.Domain.Catalog.Enums;
using SneakersShop.Domain.Common.Entities;
using SneakersShop.Domain.Common.Guards;

namespace SneakersShop.Domain.Catalog;

public sealed class Product : AggregateRoot
{
    public Guid BrandId { get; private init; }
    public Guid CategoryId { get; private init; }
    public Gender Gender { get; private set; }
    public string Model { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public decimal BasePrice { get; private set; }
    public bool IsActive { get; private set; }

    private Product() { }

    private Product(
        Guid id,
        Guid brandId,
        Guid categoryId,
        Gender gender,
        string model,
        string description,
        decimal basePrice)
        : base(id)
    {
        BrandId = brandId;
        CategoryId = categoryId;
        Gender = gender;
        Model = model;
        Description = description;
        BasePrice = basePrice;
        IsActive = true;
    }

    public static Product Create(
        Guid brandId,
        Guid categoryId,
        Gender gender,
        string model,
        string description,
        decimal basePrice)
    {
        Guard.Against.Empty(brandId);
        Guard.Against.Empty(categoryId);
        Guard.Against.NullOrWhiteSpace(model);
        Guard.Against.NullOrWhiteSpace(description);
        Guard.Against.NegativeOrZero(basePrice);

        return new Product(Guid.CreateVersion7(), brandId, categoryId, gender, model, description, basePrice);
    }

    public void Activate()
    {
        if (IsActive) return;
        IsActive = true;
        AddDomainEvent(new ProductActivated(Id));
        Touch();
    }

    public void Deactivate()
    {
        if (!IsActive) return;
        IsActive = false;
        AddDomainEvent(new ProductDeactivated(Id));
        Touch();
    }
}