using SneakersShop.Domain.Common.Results;

namespace SneakersShop.Domain.Warehouse.Errors;

public static class WarehouseError
{
    public static Error QuantityExceedsMaximum(int quantity, int maximum) =>
        Error.Validation("Warehouse.QuantityExceedsMaximum", $"Quantity ({quantity}) exceeds the maximum allowed ({maximum}).");
    public static Error InsufficientStock(int requested, int available) =>
        Error.Validation("Warehouse.InsufficientStock", $"Requested quantity ({requested}) exceeds available stock ({available}).");
    public static Error ItemNotFound(Guid itemId) =>
        Error.NotFound("Warehouse.ItemNotFound", $"Warehouse item with ID '{itemId}' was not found.");
}