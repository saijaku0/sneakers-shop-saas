using SneakersShop.Domain.Common.Results;

namespace SneakersShop.Domain.Consumer.Errors;

public static class UserErrors
{
    public static Error UserIsUnauthorized =>
        Error.Unauthorized("user.unauthorized", "The user is not authorized in the system.");
}