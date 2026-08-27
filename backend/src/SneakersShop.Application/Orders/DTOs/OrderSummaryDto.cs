namespace SneakersShop.Application.Orders.DTOs;

public sealed record OrderSummaryDto(
    Guid Id,
    string Status,
    decimal TotalAmount,
    DateTimeOffset CreatedAt,
    int ItemCount,
    IReadOnlyList<string> PreviewImages,
    string ItemsPreviewText);