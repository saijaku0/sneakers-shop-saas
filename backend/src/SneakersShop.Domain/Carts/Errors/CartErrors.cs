using SneakersShop.Domain.Common.Results;

namespace SneakersShop.Domain.Carts.Errors;

public static class CartErrors
{
    public static Error CartNotFound =>
        Error.NotFound("cart.notFound", "The cart for the specified user was not found.");

}