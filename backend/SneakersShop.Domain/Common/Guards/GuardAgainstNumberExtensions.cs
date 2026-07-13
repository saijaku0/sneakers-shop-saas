using System.Runtime.CompilerServices;

namespace SneakersShop.Domain.Common.Guards;
public static class GuardAgainstNumberExtensions
{
    public static int NegativeOrZero(
        this IGuard guard,
        int value,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        if (value <= 0)
            throw new ArgumentException($"Parameter '{parameterName}' must be greater than zero.", parameterName);
        return value;
    }
}
