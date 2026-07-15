namespace SneakersShop.Domain.Common.Guards;

/// <summary>
/// Specifies a contract for guard implementations that can be used to validate conditions and throw exceptions if the conditions are not met.
/// </summary>
public interface IGuard { }

/// <summary>
/// Represents a guard that can be used to validate conditions and throw exceptions if the conditions are not met.
/// </summary>
public sealed class Guard : IGuard
{
    /// <summary>
    /// Gets a singleton instance of the Guard class, which can be used to validate conditions and throw exceptions if the conditions are not met.
    /// </summary>
    public static IGuard Against { get; } = new Guard();
    private Guard() { }
}