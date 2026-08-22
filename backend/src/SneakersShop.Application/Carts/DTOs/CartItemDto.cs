namespace SneakersShop.Application.Carts.DTOs;

public sealed record CartItemDto(
    Guid WarehouseItemId,
    Guid ProductId,
    string Model,
    string BrandName,
    string Color,
    decimal SizeCm,
    string PreviewImageUrl,
    decimal UnitPrice,
    int Quantity,
    int Available,
    bool IsAvailable);