using NumeriCall.MathOps;


Console.Write("Enter expression: ");
string expression = Console.ReadLine() ?? string.Empty;

if (string.IsNullOrEmpty(expression)) throw new Exception("Empty expression");
Console.WriteLine($"Result: {Calculator.Calculate(expression)}");