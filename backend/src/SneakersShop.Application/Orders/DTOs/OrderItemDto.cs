namespace SneakersShop.Application.Orders.DTOs;

public sealed record OrderItemDto(
    Guid WarehouseItemId,
    string Model,
    string BrandName,
    string Color,
    decimal SizeCm,
    string PreviewImageUrl,
    decimal UnitPrice,
    int Quantity,
    decimal DiscountAmount,
    decimal TotalPrice);