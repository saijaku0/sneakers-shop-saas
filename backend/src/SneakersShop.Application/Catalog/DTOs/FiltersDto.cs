namespace SneakersShop.Application.Catalog.DTOs;

public sealed record FiltersDto(
    IReadOnlyList<string> Brands,
    IReadOnlyList<string> Categories,
    IReadOnlyList<string> Colors,
    IReadOnlyList<decimal> Sizes,
    PriceRangeDto PriceRange);