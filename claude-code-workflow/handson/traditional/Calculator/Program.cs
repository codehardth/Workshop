// Traditional Calculator - Demonstrating realistic-looking code with hidden bugs
// Simple class-based implementation without comprehensive error handling

var calculator = new Calculator();

Console.WriteLine("=================================");
Console.WriteLine("  Traditional Calculator");
Console.WriteLine("=================================");

while (true)
{
    Console.WriteLine();
    Console.WriteLine("Select operation:");
    Console.WriteLine("1. Add");
    Console.WriteLine("2. Subtract");
    Console.WriteLine("3. Multiply");
    Console.WriteLine("4. Divide");
    Console.WriteLine("5. Square Root");
    Console.WriteLine("6. Power");
    Console.WriteLine("7. Modulo");
    Console.WriteLine("0. Exit");
    Console.Write("Choice: ");

    var choice = Console.ReadLine();

    if (choice == "0")
    {
        Console.WriteLine("Goodbye!");
        break;
    }

    double a, b, result;

    switch (choice)
    {
        case "1":
            Console.Write("Enter first number: ");
            a = double.Parse(Console.ReadLine()!);
            Console.Write("Enter second number: ");
            b = double.Parse(Console.ReadLine()!);
            result = calculator.Add(a, b);
            Console.WriteLine($"Result: {a} + {b} = {result}");
            break;

        case "2":
            Console.Write("Enter first number: ");
            a = double.Parse(Console.ReadLine()!);
            Console.Write("Enter second number: ");
            b = double.Parse(Console.ReadLine()!);
            result = calculator.Subtract(a, b);
            Console.WriteLine($"Result: {a} - {b} = {result}");
            break;

        case "3":
            Console.Write("Enter first number: ");
            a = double.Parse(Console.ReadLine()!);
            Console.Write("Enter second number: ");
            b = double.Parse(Console.ReadLine()!);
            result = calculator.Multiply(a, b);
            Console.WriteLine($"Result: {a} * {b} = {result}");
            break;

        case "4":
            Console.Write("Enter dividend: ");
            a = double.Parse(Console.ReadLine()!);
            Console.Write("Enter divisor: ");
            b = double.Parse(Console.ReadLine()!);
            result = calculator.Divide(a, b);
            Console.WriteLine($"Result: {a} / {b} = {result}");
            break;

        case "5":
            Console.Write("Enter number: ");
            a = double.Parse(Console.ReadLine()!);
            result = calculator.SquareRoot(a);
            Console.WriteLine($"Result: sqrt({a}) = {result}");
            break;

        case "6":
            Console.Write("Enter base: ");
            a = double.Parse(Console.ReadLine()!);
            Console.Write("Enter exponent: ");
            b = double.Parse(Console.ReadLine()!);
            result = calculator.Power(a, b);
            Console.WriteLine($"Result: {a} ^ {b} = {result}");
            break;

        case "7":
            Console.Write("Enter dividend: ");
            a = double.Parse(Console.ReadLine()!);
            Console.Write("Enter divisor: ");
            b = double.Parse(Console.ReadLine()!);
            result = calculator.Modulo(a, b);
            Console.WriteLine($"Result: {a} % {b} = {result}");
            break;

        default:
            Console.WriteLine("Invalid choice");
            break;
    }
}