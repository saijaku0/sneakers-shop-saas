using SneakersShop.Domain.Catalog.ValueObjects;
using SneakersShop.Domain.Common.Entities;
using SneakersShop.Domain.Common.Guards;

namespace SneakersShop.Domain.Catalog;

public sealed class ProductVariant : AggregateRoot
{
    private readonly List<ProductImage> _images = [];
    public IReadOnlyCollection<ProductImage> Images => _images.AsReadOnly();

    public Guid ProductId { get; private init; }
    public string Color { get; private set; } = null!;

    private ProductVariant() { }

    private ProductVariant(Guid id, Guid productId, string color, IEnumerable<ProductImage> images)
        : base(id)
    {
        ProductId = productId;
        Color = color;
        _images.AddRange(images);
    }

    public static ProductVariant Create(Guid productId, string color, IEnumerable<ProductImage> images)
    {
        Guard.Against.Empty(productId);
        Guard.Against.NullOrWhiteSpace(color);
        Guard.Against.NullOrEmpty(images);

        return new ProductVariant(Guid.CreateVersion7(), productId, color, images);
    }

    public void UpdateColor(string color)
    {
        Guard.Against.NullOrWhiteSpace(color);
        Color = color;
        Touch();
    }

    public void SetImages(IEnumerable<ProductImage> images)
    {
        Guard.Against.NullOrEmpty(images);
        _images.Clear();
        _images.AddRange(images);
        Touch();
    }
}