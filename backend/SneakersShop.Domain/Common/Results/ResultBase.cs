namespace SneakersShop.Domain.Common.Results;

/// <summary>
/// Represents the base class for results, indicating success or failure and carrying an optional error.
/// </summary>
public class ResultBase
{
    public Error? Error { get; }
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    protected ResultBase(bool isSuccess, Error? error)
    {
        if (isSuccess && error != Error.None)
            throw new InvalidOperationException("A successful result cannot carry an error.");
        if (!isSuccess && error == Error.None)
            throw new InvalidOperationException("A failed result must carry an error.");

        IsSuccess = isSuccess;
        Error = error;
    }
}