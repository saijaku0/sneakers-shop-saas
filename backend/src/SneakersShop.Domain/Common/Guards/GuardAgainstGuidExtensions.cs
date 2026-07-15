using System.Runtime.CompilerServices;

namespace SneakersShop.Domain.Common.Guards;

public static class GuardAgainstGuidExtensions
{
    public static Guid Empty(
        this IGuard guard,
        Guid value,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        if (value == Guid.Empty)
            throw new ArgumentException($"Parameter '{parameterName}' cannot be an empty GUID.", parameterName);
        return value;
    }
}
