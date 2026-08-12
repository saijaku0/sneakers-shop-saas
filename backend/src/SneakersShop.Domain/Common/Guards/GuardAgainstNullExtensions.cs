using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace SneakersShop.Domain.Common.Guards;

public static class GuardAgainstNullExtensions
{
    public static string NullOrEmpty(
    this IGuard guard,
    [NotNull] string? value,
    [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        if (string.IsNullOrEmpty(value))
            throw new ArgumentException($"Parameter '{parameterName}' cannot be null or empty.", parameterName);
        return value;
    }

    public static IEnumerable<T> NullOrEmpty<T>(
        this IGuard guard,
        [NotNull] IEnumerable<T>? value,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        if (value is null || !value.Any())
            throw new ArgumentException($"Parameter '{parameterName}' cannot be null or empty.", parameterName);
        return value;
    }

    public static string NullOrWhiteSpace(
        this IGuard guard,
        [NotNull] string? value,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"Parameter '{parameterName}' cannot be null or whitespace.", parameterName);
        return value;
    }

    public static T Null<T>(
        this IGuard guard,
        [NotNull] T? value,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
        where T : class
    {
        if (value is null)
            throw new ArgumentNullException(parameterName, $"Parameter '{parameterName}' cannot be null.");
        return value;
    }

    public static T Default<T>(
        this IGuard guard,
        [AllowNull, NotNull] T value,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
        where T : struct
    {
        if (EqualityComparer<T>.Default.Equals(value, default!))
            throw new ArgumentException($"Parameter '{parameterName}' cannot be the default value.", parameterName);
        return value;
    }
}