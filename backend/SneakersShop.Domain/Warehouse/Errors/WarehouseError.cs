using SneakersShop.Domain.Common.Results;

namespace SneakersShop.Domain.Warehouse.Errors;

public static class WarehouseError
{
    public static Error ProductNotFound(Guid productId) =>
        Error.NotFound("Warehouse.ProductNotFound", $"Product with ID '{productId}' was not found in the warehouse.");

    public static Error SizeIsNull() =>
        Error.Validation("Warehouse.SizeIsNull", "Size cannot be null.");

    public static Error QuantityExceedsMaximum(int quantity, int maximum) =>
        Error.Validation("Warehouse.QuantityExceedsMaximum", $"Quantity ({quantity}) exceeds the maximum allowed ({maximum}).");
}
