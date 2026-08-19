using SneakersShop.Domain.Common.Results;

namespace SneakersShop.Domain.Catalog.Errors;

public static class ProductErrors
{
    public static Error ProductNotFound(Guid id) =>
        Error.NotFound("Product.NotFound", $"Product '{id}' was not found.");
}