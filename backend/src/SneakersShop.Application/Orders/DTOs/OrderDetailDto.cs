using SneakersShop.Application.Profiles.DTOs;

namespace SneakersShop.Application.Orders.DTOs;

public sealed record OrderDetailDto(
    Guid Id,
    string Status,
    decimal TotalAmount,
    string PaymentMethod,
    DateTimeOffset PaymentDeadline,
    DateTimeOffset CreatedAt,
    AddressDto ShippingAddress,
    IReadOnlyList<OrderItemDto> Items);