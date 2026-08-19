namespace SneakersShop.Application.Catalog.DTOs;

public record ProductListItem(
    Guid ProductId,
    string Model,
    string BrandName,
    decimal BasePrice,
    IReadOnlyList<ProductVariantPreviewDto> Variants,
    int ColorCount);