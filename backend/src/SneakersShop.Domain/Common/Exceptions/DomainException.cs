namespace SneakersShop.Domain.Common.Exceptions;

[Serializable]
public class DomainException : Exception
{
    public string? ParamName { get; }
    public string? ErrorCode { get; }

    public DomainException() { }
    public DomainException(string message) : base(message) { }

    public DomainException(string? message, Exception? innerException)
        : base(message, innerException)
    { }

    public DomainException(string? message, string? paramName)
        : base(message)
    {
        ParamName = paramName;
    }

    public DomainException(string? message, string? paramName, string? errorCode)
        : base(message)
    {
        ParamName = paramName;
        ErrorCode = errorCode;
    }
}