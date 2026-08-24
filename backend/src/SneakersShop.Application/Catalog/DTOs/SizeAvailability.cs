namespace SneakersShop.Application.Catalog.DTOs;

public sealed record SizeAvailability(Guid WarehouseItemId, decimal SizeCm, bool InStock);