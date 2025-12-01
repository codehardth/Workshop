namespace Calculator;

/// <summary>
/// Implementation of ICalculator with proper validation and error handling.
/// Uses the Result pattern to communicate success and failure cases.
/// </summary>
public class Calculator : ICalculator
{
    /// <inheritdoc />
    public Result<double> Add(double a, double b)
    {
        return Result<double>.Success(a + b);
    }

    /// <inheritdoc />
    public Result<double> Subtract(double a, double b)
    {
        return Result<double>.Success(a - b);
    }

    /// <inheritdoc />
    public Result<double> Multiply(double a, double b)
    {
        return Result<double>.Success(a * b);
    }

    /// <inheritdoc />
    public Result<double> Divide(double a, double b)
    {
        if (b == 0)
        {
            return Result<double>.Failure("Cannot divide by zero.");
        }

        return Result<double>.Success(a / b);
    }

    /// <inheritdoc />
    public Result<double> SquareRoot(double n)
    {
        if (n < 0)
        {
            return Result<double>.Failure("Cannot calculate square root of a negative number.");
        }

        return Result<double>.Success(Math.Sqrt(n));
    }

    /// <inheritdoc />
    public Result<double> Power(double baseNum, double exponent)
    {
        return Result<double>.Success(Math.Pow(baseNum, exponent));
    }

    /// <inheritdoc />
    public Result<double> Modulo(double a, double b)
    {
        if (b == 0)
        {
            return Result<double>.Failure("Cannot perform modulo by zero.");
        }

        return Result<double>.Success(a % b);
    }
}
