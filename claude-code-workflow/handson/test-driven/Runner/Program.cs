namespace Runner;

using Calculator;

/// <summary>
/// REPL (Read-Eval-Print Loop) for the expression calculator.
/// Provides an interactive command-line interface for evaluating mathematical expressions.
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        PrintWelcome();
        RunRepl();
        Console.WriteLine("Goodbye!");
    }

    /// <summary>
    /// Displays the welcome message.
    /// </summary>
    static void PrintWelcome()
    {
        Console.WriteLine("Expression Calculator");
        Console.WriteLine("Type 'help' for commands, 'exit' to quit.");
        Console.WriteLine();
    }

    /// <summary>
    /// Runs the main REPL loop.
    /// </summary>
    static void RunRepl()
    {
        while (true)
        {
            Console.Write("> ");
            var input = Console.ReadLine();

            // Handle null input (Ctrl+D / end of stream)
            if (input == null)
            {
                Console.WriteLine();
                break;
            }

            // Trim whitespace
            input = input.Trim();

            // Handle empty input
            if (string.IsNullOrEmpty(input))
            {
                continue;
            }

            // Handle commands
            var lowerInput = input.ToLowerInvariant();

            if (lowerInput == "exit")
            {
                break;
            }

            if (lowerInput == "help")
            {
                PrintHelp();
                continue;
            }

            if (lowerInput == "clear")
            {
                Console.Clear();
                continue;
            }

            // Evaluate expression
            EvaluateAndPrint(input);
        }
    }

    /// <summary>
    /// Displays the help message.
    /// </summary>
    static void PrintHelp()
    {
        Console.WriteLine("Commands:");
        Console.WriteLine("  <expression>  Evaluate a mathematical expression");
        Console.WriteLine("  help          Show this help message");
        Console.WriteLine("  clear         Clear the screen");
        Console.WriteLine("  exit          Exit the calculator");
    }

    /// <summary>
    /// Evaluates an expression and prints the result or error.
    /// Pipeline: tokenize -> parse -> evaluate -> display
    /// Uses the Result.Bind method for clean chaining.
    /// </summary>
    private static void EvaluateAndPrint(string input)
    {
        // Chain: Tokenize -> Parse -> Evaluate
        var result =
            Lexer.Tokenize(input)
                 .Bind(Parser.Parse)
                 .Bind(Evaluator.Evaluate);

        result.Match(
            value =>
            {
                // Format the output: show integer for whole numbers, otherwise show decimal
                if (value.Equals(Math.Floor(value)) && !double.IsInfinity(value) && Math.Abs(value) < 1e15)
                {
                    Console.WriteLine((long)value);
                }
                else
                {
                    Console.WriteLine(value);
                }

                return 0;
            },
            error =>
            {
                Console.WriteLine($"Error: {error}");
                return 0;
            }
        );
    }
}