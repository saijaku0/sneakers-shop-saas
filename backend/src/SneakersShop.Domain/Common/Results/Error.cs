namespace SneakersShop.Domain.Common.Results;

public enum ErrorType
{
    Validation,
    NotFound,
    Conflict,
    Unauthorized,
    Forbidden,
    Internal,
    BadRequest,
    TooManyRequests,
    Invalid,
    None
}

/// <summary>
/// Represents an error that can occur during the execution of an operation, including a code, message, and type.
/// </summary>
/// <param name="Code">The code identifying the error.</param>
/// <param name="Message">The message describing the error.</param>
/// <param name="Type">The type of the error.</param>
public record Error(string Code, string Message, ErrorType Type)
{
    public static Error NotFound(string code, string message) =>
        new(code, message, ErrorType.NotFound);
    public static Error Validation(string code, string message)
        => new(code, message, ErrorType.Validation);
    public static Error Unauthorized(string code, string message)
        => new(code, message, ErrorType.Unauthorized);
    public static Error Internal(string code, string message)
        => new(code, message, ErrorType.Internal);
    public static Error Conflict(string code, string message)
        => new(code, message, ErrorType.Conflict);
    public static Error BadRequest(string code, string message)
        => new(code, message, ErrorType.BadRequest);
    public static Error Forbidden(string code, string message)
        => new(code, message, ErrorType.Forbidden);
    public static Error TooManyRequests(string code, string message)
        => new(code, message, ErrorType.TooManyRequests);
    public static Error Invalid(string code, string message)
        => new(code, message, ErrorType.Invalid);

    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.None);
}