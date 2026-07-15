using SneakersShop.Domain.Catalog.DomainEvents;
using SneakersShop.Domain.Catalog.ValueObjects;
using SneakersShop.Domain.Common.Entities;
using SneakersShop.Domain.Common.Guards;

namespace SneakersShop.Domain.Catalog;

/* TODO: Update model 
 * add property: Color;
 * 
 * Need think about price is decimal or Money value object;
 * 
 * braking changes: Color, need update UML view and database schema
*/

public sealed class Product : AggregateRoot
{
    public Guid BrandId { get; private init; }
    public string Model { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public decimal Price { get; private set; }
    public bool IsActive { get; private set; }
    public IReadOnlyCollection<ProductImage> Images => _images.AsReadOnly();
    private readonly List<ProductImage> _images = [];

    private Product() { }
    private Product(
        Guid brandId,
        string model,
        string description,
        decimal price,
        IEnumerable<ProductImage> images) : base(Guid.CreateVersion7())
    {
        BrandId = brandId;
        Model = model;
        Description = description;
        Price = price;
        _images.AddRange(images);

        IsActive = true;
    }

    public static Product Create(
    Guid brandId,
    string model,
    string description,
    decimal price,
    IEnumerable<ProductImage> images)
    {
        Guard.Against.Empty(brandId);
        Guard.Against.NullOrWhiteSpace(model);
        Guard.Against.NullOrWhiteSpace(description);
        Guard.Against.NegativeOrZero(price);
        Guard.Against.Null(images);

        return new Product(brandId, model, description, price, images);
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
