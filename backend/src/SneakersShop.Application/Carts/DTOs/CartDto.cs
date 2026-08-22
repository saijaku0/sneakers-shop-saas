namespace SneakersShop.Application.Carts.DTOs;

public sealed record CartDto(
    IReadOnlyList<CartItemDto> Items,
    decimal TotalPrice);