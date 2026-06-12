class CalculatorApp
{
    static void Main()
    {
        Console.WriteLine("Welcome!");

        while (true)
        {
            Console.WriteLine("Input ('done' to quit, or 'help' for help):");

            string input = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(input.IsNull())) continue;

            if (input == "done") break;

            if (input.Equals("help", StringComparison.OrdinalIgnoreCase) || input == "?")
            {
                Console.WriteLine("Help:");
                Console.WriteLine("  Enter an expression as three space-separated values: <left> <op> <right>");
                Console.WriteLine("  Supported operations: +, -, *, /.");
                Console.WriteLine("  Examples: 5 + 3   or   10 / 2");
                Console.WriteLine("  Type 'done' to exit the program.");
                continue;
            }

            string[] symbols = input.Split(' ');
            if (symbols.Length != 3)
            {
                Console.WriteLine("Invalid expression. Expected an expression with a single operation and two operands.");
                continue;
            }

            if (!double.TryParse(symbols[0], out double left_value))
            {
                Console.WriteLine($"Invalid expression. Operand {symbols[0]} is invalid.");
                continue;
            }
            if (!double.TryParse(symbols[2], out double right_value))
            {
                Console.WriteLine($"Invalid expression. Operand {symbols[2]} is invalid.");
                continue;
            }

            string operation = symbols[1];
            switch (operation)
            {
                case "+": Console.WriteLine($"{left_value + right_value}"); break;
                case "-": Console.WriteLine($"{left_value - right_value}"); break;
                case "*": Console.WriteLine($"{left_value * right_value}"); break;
                case "/":
                    Console.WriteLine(right_value == 0 ? "Division by zero encountered." : $"{left_value / right_value}");
                    break;
                default: Console.WriteLine($"Operation {operation} not recognized."); break;
            }
        }

        Console.WriteLine("Goodbye!");
    }
}
