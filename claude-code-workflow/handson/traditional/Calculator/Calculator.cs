public class Calculator
{
    public double Add(double a, double b) => a + b;

    public double Subtract(double a, double b) => a - b;

    public double Multiply(double a, double b) => a * b;

    public double Divide(double a, double b) => a / b; // Hidden bug: returns Infinity when b is 0

    public double SquareRoot(double n) => Math.Sqrt(n); // Hidden bug: returns NaN for negative numbers

    public double Power(double baseNum, double exponent) => Math.Pow(baseNum, exponent);

    public double Modulo(double a, double b) => a % b; // Hidden bug: returns NaN when b is 0
}