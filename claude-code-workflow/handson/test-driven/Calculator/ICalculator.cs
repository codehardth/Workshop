namespace Calculator;

/// <summary>
/// Defines the contract for calculator operations.
/// All operations return a Result to handle both success and error cases.
/// </summary>
public interface ICalculator
{
    /// <summary>
    /// Adds two numbers together.
    /// </summary>
    /// <param name="a">The first operand</param>
    /// <param name="b">The second operand</param>
    /// <returns>Result containing the sum or an error</returns>
    Result<double> Add(double a, double b);

    /// <summary>
    /// Subtracts the second number from the first.
    /// </summary>
    /// <param name="a">The minuend</param>
    /// <param name="b">The subtrahend</param>
    /// <returns>Result containing the difference or an error</returns>
    Result<double> Subtract(double a, double b);

    /// <summary>
    /// Multiplies two numbers together.
    /// </summary>
    /// <param name="a">The first factor</param>
    /// <param name="b">The second factor</param>
    /// <returns>Result containing the product or an error</returns>
    Result<double> Multiply(double a, double b);

    /// <summary>
    /// Divides the first number by the second.
    /// </summary>
    /// <param name="a">The dividend</param>
    /// <param name="b">The divisor</param>
    /// <returns>Result containing the quotient or an error if divisor is zero</returns>
    Result<double> Divide(double a, double b);

    /// <summary>
    /// Calculates the square root of a number.
    /// </summary>
    /// <param name="n">The number to find the square root of</param>
    /// <returns>Result containing the square root or an error if number is negative</returns>
    Result<double> SquareRoot(double n);

    /// <summary>
    /// Raises a base number to an exponent.
    /// </summary>
    /// <param name="baseNum">The base number</param>
    /// <param name="exponent">The exponent</param>
    /// <returns>Result containing the power or an error</returns>
    Result<double> Power(double baseNum, double exponent);

    /// <summary>
    /// Calculates the modulo (remainder) of division.
    /// </summary>
    /// <param name="a">The dividend</param>
    /// <param name="b">The divisor</param>
    /// <returns>Result containing the remainder or an error if divisor is zero</returns>
    Result<double> Modulo(double a, double b);
}
