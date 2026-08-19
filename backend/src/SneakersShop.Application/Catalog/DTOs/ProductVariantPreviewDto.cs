namespace SneakersShop.Application.Catalog.DTOs;

public sealed record ProductVariantPreviewDto(
    Guid VariantId,
    string ColorName,
    string ImageUrl);