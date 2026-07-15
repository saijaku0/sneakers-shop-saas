using System.Runtime.CompilerServices;

namespace SneakersShop.Domain.Common.Guards;

public static class GuardAgainstNumberExtensions
{
    public static T Negative<T>(
        this IGuard guard,
        T value,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
        where T : struct, IComparable<T>
    {
        if (value.CompareTo(default) < 0)
            throw new ArgumentException($"Parameter '{parameterName}' must be non-negative.", parameterName);
        return value;
    }

    public static T NegativeOrZero<T>(
        this IGuard guard,
        T value,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
        where T : struct, IComparable<T>
    {
        if (value.CompareTo(default) <= 0)
            throw new ArgumentException($"Parameter '{parameterName}' must be greater than zero.", parameterName);
        return value;
    }
}