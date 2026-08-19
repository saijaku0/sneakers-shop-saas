namespace SneakersShop.Application.Catalog.DTOs;

public sealed record VariantDetail(
    Guid VariantId,
    string ColorName,
    IReadOnlyList<string> Images,
    IReadOnlyList<SizeAvailability> Sizes);