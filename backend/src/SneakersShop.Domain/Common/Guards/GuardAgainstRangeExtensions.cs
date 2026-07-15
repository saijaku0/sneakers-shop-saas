using System.Runtime.CompilerServices;

namespace SneakersShop.Domain.Common.Guards;
public static class GuardAgainstRangeExtensions
{
    public static int OutOfRange(
        this IGuard guard,
        int value,
        int minValue,
        int maxValue,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        if (value < minValue || value > maxValue)
            throw new ArgumentOutOfRangeException(parameterName, $"Parameter '{parameterName}' must be in the range [{minValue}, {maxValue}].");
        return value;
    }
}
