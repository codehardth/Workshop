namespace Calculator;

/// <summary>
/// Represents the result of an operation that can either succeed with a value
/// or fail with an error message. This is a discriminated union pattern.
/// </summary>
/// <typeparam name="T">The type of the success value</typeparam>
public abstract record Result<T>
{
    // Private constructor prevents external inheritance
    private Result() { }

    /// <summary>
    /// Represents a successful result containing a value.
    /// </summary>
    public sealed record Success(T Value) : Result<T>;

    /// <summary>
    /// Represents a failed result containing an error message.
    /// </summary>
    public sealed record Failure(string Error) : Result<T>;

    /// <summary>
    /// Returns true if this result is a success.
    /// </summary>
    public bool IsSuccess => this is Success;

    /// <summary>
    /// Returns true if this result is a failure.
    /// </summary>
    public bool IsFailure => this is Failure;

    /// <summary>
    /// Creates a successful result with the given value.
    /// </summary>
    public static Result<T> Ok(T value) => new Success(value);

    /// <summary>
    /// Creates a failed result with the given error message.
    /// </summary>
    public static Result<T> Fail(string error) => new Failure(error);

    /// <summary>
    /// Executes the appropriate function based on success or failure and returns the result.
    /// </summary>
    /// <typeparam name="TResult">The return type of the match functions</typeparam>
    /// <param name="onSuccess">Function to execute if this is a success</param>
    /// <param name="onFailure">Function to execute if this is a failure</param>
    /// <returns>The result of the executed function</returns>
    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<string, TResult> onFailure)
    {
        return this switch
        {
            Success s => onSuccess(s.Value),
            Failure f => onFailure(f.Error),
            _ => throw new InvalidOperationException("Unexpected result type")
        };
    }

    /// <summary>
    /// Executes the appropriate action based on success or failure.
    /// </summary>
    /// <param name="onSuccess">Action to execute if this is a success</param>
    /// <param name="onFailure">Action to execute if this is a failure</param>
    public void Match(Action<T> onSuccess, Action<string> onFailure)
    {
        switch (this)
        {
            case Success s:
                onSuccess(s.Value);
                break;
            case Failure f:
                onFailure(f.Error);
                break;
        }
    }
}
