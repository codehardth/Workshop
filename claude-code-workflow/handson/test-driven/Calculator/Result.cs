namespace Calculator;

/// <summary>
/// A discriminated union representing either a successful result or a failure with an error message.
/// Used throughout the calculator pipeline for error handling without exceptions.
/// </summary>
/// <typeparam name="T">The type of the success value</typeparam>
public abstract record Result<T>
{
    /// <summary>
    /// Represents a successful result containing a value.
    /// </summary>
    public sealed record Success(T Value) : Result<T>;

    /// <summary>
    /// Represents a failure result containing an error message.
    /// </summary>
    public sealed record Failure(string Error) : Result<T>;

    // Prevent external inheritance
    private Result() { }

    /// <summary>
    /// Returns true if this result represents a success.
    /// </summary>
    public bool IsSuccess => this is Success;

    /// <summary>
    /// Returns true if this result represents a failure.
    /// </summary>
    public bool IsFailure => this is Failure;

    /// <summary>
    /// Creates a successful result with the given value.
    /// </summary>
    public static Result<T> Ok(T value) => new Success(value);

    /// <summary>
    /// Creates a failure result with the given error message.
    /// </summary>
    public static Result<T> Fail(string error) => new Failure(error);

    /// <summary>
    /// Matches against the result type and executes the appropriate function.
    /// Enables exhaustive pattern matching on success/failure cases.
    /// </summary>
    /// <typeparam name="TResult">The return type of the match functions</typeparam>
    /// <param name="onSuccess">Function to execute if this is a Success</param>
    /// <param name="onFailure">Function to execute if this is a Failure</param>
    /// <returns>The result of the executed function</returns>
    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<string, TResult> onFailure)
    {
        return this switch
        {
            Success s => onSuccess(s.Value),
            Failure f => onFailure(f.Error),
            _ => throw new InvalidOperationException("Unexpected Result type")
        };
    }

    /// <summary>
    /// Maps a successful result to a new type using the provided function.
    /// If this is a failure, the error is propagated.
    /// </summary>
    public Result<TNew> Map<TNew>(Func<T, TNew> mapper)
    {
        return this switch
        {
            Success s => Result<TNew>.Ok(mapper(s.Value)),
            Failure f => Result<TNew>.Fail(f.Error),
            _ => throw new InvalidOperationException("Unexpected Result type")
        };
    }

    /// <summary>
    /// Chains a result-producing operation onto a successful result.
    /// If this is a failure, the error is propagated.
    /// </summary>
    public Result<TNew> Bind<TNew>(Func<T, Result<TNew>> binder)
    {
        return this switch
        {
            Success s => binder(s.Value),
            Failure f => Result<TNew>.Fail(f.Error),
            _ => throw new InvalidOperationException("Unexpected Result type")
        };
    }
}
