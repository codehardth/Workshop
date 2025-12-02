namespace Runner;

using Calculator;

/// <summary>
/// REPL (Read-Eval-Print Loop) for the expression calculator.
/// Provides an interactive command-line interface for evaluating mathematical expressions.
/// </summary>
public class Program
{
    public static void Main(string[] args)
    {
        PrintWelcome();
        RunRepl();
        Console.WriteLine("Goodbye!");
    }

    /// <summary>
    /// Displays the welcome message.
    /// </summary>
    private static void PrintWelcome()
    {
        Console.WriteLine("Expression Calculator");
        Console.WriteLine("Type 'help' for commands, 'exit' to quit.");
        Console.WriteLine();
    }

    /// <summary>
    /// Runs the main REPL loop.
    /// </summary>
    private static void RunRepl()
    {
        while (true)
        {
            Console.Write("> ");
            string? input = Console.ReadLine();

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
            string lowerInput = input.ToLowerInvariant();

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
    private static void PrintHelp()
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
    /// </summary>
    private static void EvaluateAndPrint(string input)
    {
        // Step 1: Tokenize
        var tokenResult = Lexer.Tokenize(input);

        if (tokenResult.IsFailure)
        {
            Console.WriteLine($"Error: {((Result<List<Token>>.Failure)tokenResult).Error}");
            return;
        }

        var tokens = ((Result<List<Token>>.Success)tokenResult).Value;

        // Step 2: Parse
        var parseResult = Parser.Parse(tokens);

        if (parseResult.IsFailure)
        {
            Console.WriteLine($"Error: {((Result<Expression>.Failure)parseResult).Error}");
            return;
        }

        var expression = ((Result<Expression>.Success)parseResult).Value;

        // Step 3: Evaluate
        var evalResult = Evaluator.Evaluate(expression);

        evalResult.Match(
            onSuccess: value =>
            {
                // Format the output: show integer for whole numbers, otherwise show decimal
                if (value == Math.Floor(value) && Math.Abs(value) < 1e15)
                {
                    Console.WriteLine((long)value);
                }
                else
                {
                    Console.WriteLine(value);
                }
            },
            onFailure: error =>
            {
                Console.WriteLine($"Error: {error}");
            }
        );
    }
}