using SneakersShop.Domain.Catalog.Enums;

namespace SneakersShop.Application.Catalog.DTOs;

public sealed record ProductDetail(
    Guid Id,
    string Model,
    string BrandName,
    string CategoryName,
    Gender Gender,
    decimal BasePrice,
    string Description,
    IReadOnlyList<VariantDetail> Variants);