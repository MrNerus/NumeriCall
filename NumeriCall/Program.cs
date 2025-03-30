using NumeriCall.Logics;


Console.Write("Enter expression: ");
string expression = Console.ReadLine() ?? string.Empty;

if (string.IsNullOrEmpty(expression)) throw new Exception("Emptu expression");
Console.WriteLine($"Result: {Calculator.Calculate(expression)}");