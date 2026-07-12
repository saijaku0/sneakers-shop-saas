namespace SneakersShop.Domain.Common.Results;

/// <summary>
/// Represents the result of an operation, containing either a success state or an error.
/// </summary>
/// <remarks>
/// Use Result.Success() for a successful result and Result.Failure(Error) for a failure.
/// Supports implicit conversion from Error to Result for convenient error returns.
/// </remarks>
public class Result : ResultBase
{
    private Result(bool isSuccess, Error? error)
        : base(isSuccess, error) { }

    public static Result Success() => new(true, Error.None);
    public static Result Failure(Error error) => new(false, error);

    /// <summary>
    /// Implicitly converts a value of type T to a successful Result<T>.
    /// </summary>
    /// <param name="error">The reason for the failure.</param>
    public static implicit operator Result(Error error) => Failure(error);
}

/// <summary>
/// Represents the result of an operation: contains a value of type T on success or an Error on failure.
/// </summary>
/// <remarks>
/// Supports implicit conversions from T and Error.
/// Accessing Value throws an InvalidOperationException for a failed result.
/// Created via Success(T) and Failure(Error).
/// </remarks>
/// <typeparam name="T">The type of the value returned on success.</typeparam>
public class Result<T> : ResultBase
{
    private readonly T? _value;
    private Result(T value) : base(true, Error.None)
    {
        // Runtime check to ensure value is not null, as Result<T> should not hold a null value on success.
        ArgumentNullException.ThrowIfNull(value);
        _value = value; 
    }
    private Result(Error error) : base(false, error) => _value = default;

    public T Value => IsSuccess
    ? _value!
    : throw new InvalidOperationException("Cannot access Value on a failed result.");
    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(Error error) => new(error);

    /// <summary>
    /// Implicitly converts a value of type T to a successful Result<T>.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    public static implicit operator Result<T>(T value) => Success(value);

    /// <summary>
    /// Implicitly converts an Error to a failed Result<T>.
    /// </summary>
    /// <param name="error">The reason for the failure.</param>
    public static implicit operator Result<T>(Error error) => Failure(error);
}
