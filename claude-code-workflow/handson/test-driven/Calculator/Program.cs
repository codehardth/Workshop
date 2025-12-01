using Calculator;

/// <summary>
/// Interactive REPL (Read-Eval-Print-Loop) calculator application.
/// Demonstrates usage of the ICalculator interface with Result pattern.
/// </summary>
class Program
{
    private static readonly ICalculator _calculator = new Calculator.Calculator();

    static void Main(string[] args)
    {
        Console.WriteLine("==============================================");
        Console.WriteLine("   TDD Calculator - Interactive REPL");
        Console.WriteLine("==============================================");
        Console.WriteLine();
        PrintHelp();

        while (true)
        {
            Console.WriteLine();
            Console.Write("> ");
            var input = Console.ReadLine()?.Trim().ToLower();

            if (string.IsNullOrEmpty(input))
                continue;

            if (input == "exit" || input == "quit" || input == "q")
            {
                Console.WriteLine("Goodbye!");
                break;
            }

            if (input == "help" || input == "h" || input == "?")
            {
                PrintHelp();
                continue;
            }

            ProcessCommand(input);
        }
    }

    /// <summary>
    /// Prints available commands and usage instructions.
    /// </summary>
    static void PrintHelp()
    {
        Console.WriteLine("Available operations:");
        Console.WriteLine("  add <a> <b>      - Add two numbers (a + b)");
        Console.WriteLine("  sub <a> <b>      - Subtract two numbers (a - b)");
        Console.WriteLine("  mul <a> <b>      - Multiply two numbers (a * b)");
        Console.WriteLine("  div <a> <b>      - Divide two numbers (a / b)");
        Console.WriteLine("  sqrt <n>         - Square root of a number");
        Console.WriteLine("  pow <base> <exp> - Power (base ^ exponent)");
        Console.WriteLine("  mod <a> <b>      - Modulo (a % b)");
        Console.WriteLine();
        Console.WriteLine("Other commands:");
        Console.WriteLine("  help, h, ?       - Show this help");
        Console.WriteLine("  exit, quit, q    - Exit the calculator");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  > add 5 3");
        Console.WriteLine("  > sqrt 16");
        Console.WriteLine("  > pow 2 10");
    }

    /// <summary>
    /// Processes user input and executes the corresponding calculator operation.
    /// </summary>
    /// <param name="input">The user input command</param>
    static void ProcessCommand(string input)
    {
        var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 0)
            return;

        var command = parts[0];
        Result<double> result;

        try
        {
            switch (command)
            {
                case "add":
                    if (!ValidateArgs(parts, 3, "add <a> <b>")) return;
                    result = _calculator.Add(ParseDouble(parts[1]), ParseDouble(parts[2]));
                    break;

                case "sub":
                case "subtract":
                    if (!ValidateArgs(parts, 3, "sub <a> <b>")) return;
                    result = _calculator.Subtract(ParseDouble(parts[1]), ParseDouble(parts[2]));
                    break;

                case "mul":
                case "multiply":
                    if (!ValidateArgs(parts, 3, "mul <a> <b>")) return;
                    result = _calculator.Multiply(ParseDouble(parts[1]), ParseDouble(parts[2]));
                    break;

                case "div":
                case "divide":
                    if (!ValidateArgs(parts, 3, "div <a> <b>")) return;
                    result = _calculator.Divide(ParseDouble(parts[1]), ParseDouble(parts[2]));
                    break;

                case "sqrt":
                    if (!ValidateArgs(parts, 2, "sqrt <n>")) return;
                    result = _calculator.SquareRoot(ParseDouble(parts[1]));
                    break;

                case "pow":
                case "power":
                    if (!ValidateArgs(parts, 3, "pow <base> <exp>")) return;
                    result = _calculator.Power(ParseDouble(parts[1]), ParseDouble(parts[2]));
                    break;

                case "mod":
                case "modulo":
                    if (!ValidateArgs(parts, 3, "mod <a> <b>")) return;
                    result = _calculator.Modulo(ParseDouble(parts[1]), ParseDouble(parts[2]));
                    break;

                default:
                    Console.WriteLine($"Error: Unknown command '{command}'. Type 'help' for available commands.");
                    return;
            }

            PrintResult(result);
        }
        catch (FormatException)
        {
            Console.WriteLine("Error: Invalid number format. Please enter valid numbers.");
        }
        catch (OverflowException)
        {
            Console.WriteLine("Error: Number is too large or too small.");
        }
    }

    /// <summary>
    /// Validates that the correct number of arguments were provided.
    /// </summary>
    /// <param name="parts">The parsed command parts</param>
    /// <param name="expectedCount">Expected number of parts including command</param>
    /// <param name="usage">Usage string to display on error</param>
    /// <returns>True if valid, false otherwise</returns>
    static bool ValidateArgs(string[] parts, int expectedCount, string usage)
    {
        if (parts.Length != expectedCount)
        {
            Console.WriteLine($"Error: Invalid number of arguments. Usage: {usage}");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Parses a string to a double value.
    /// </summary>
    /// <param name="value">The string to parse</param>
    /// <returns>The parsed double value</returns>
    static double ParseDouble(string value)
    {
        return double.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Prints the result of a calculator operation.
    /// </summary>
    /// <param name="result">The Result object to print</param>
    static void PrintResult(Result<double> result)
    {
        if (result.IsSuccess)
        {
            Console.WriteLine($"Result: {result.Value}");
        }
        else
        {
            Console.WriteLine($"Error: {result.Error}");
        }
    }
}
