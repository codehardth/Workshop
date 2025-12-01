namespace Calculator;

/// <summary>
/// A generic Result class implementing the Result Pattern for error handling.
/// Provides a clean way to handle success and failure cases without exceptions.
/// </summary>
/// <typeparam name="T">The type of the value on success</typeparam>
public class Result<T>
{
    /// <summary>
    /// Indicates whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// The value returned on success. Default value if operation failed.
    /// </summary>
    public T Value { get; }

    /// <summary>
    /// The error message if the operation failed. Null if successful.
    /// </summary>
    public string? Error { get; }

    /// <summary>
    /// Private constructor to enforce factory method usage.
    /// </summary>
    private Result(bool isSuccess, T value, string? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    /// <summary>
    /// Creates a successful result with the given value.
    /// </summary>
    /// <param name="value">The success value</param>
    /// <returns>A successful Result containing the value</returns>
    public static Result<T> Success(T value) => new Result<T>(true, value, null);

    /// <summary>
    /// Creates a failed result with the given error message.
    /// </summary>
    /// <param name="error">The error message describing the failure</param>
    /// <returns>A failed Result containing the error message</returns>
    public static Result<T> Failure(string error) => new Result<T>(false, default!, error);
}
