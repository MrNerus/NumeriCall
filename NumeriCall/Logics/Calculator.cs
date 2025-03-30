using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace NumeriCall.Logics;

class Calculator()
{
    public static double Calculate(string input)
    {
        input = input.Replace(" ", "");
        List<string> tokens = Tokenizer.Tokenize(input);
        List<string> tokens_postFix = ConvertToPostfix(tokens);
        
        Stack<double> values = new();
        Stack<string> operators = new();
        
        double result = Evaluate(tokens_postFix, values, operators);
        return result;
    }

    public static List<string> ConvertToPostfix(List<string> tokens)
    {
        Stack<string> operators = new();
        Stack<string> output = new();

        foreach (string token in tokens)
        {

            if (
                Tokenizer.NumberRegex().IsMatch(token) ||
                Tokenizer.knownConstants.Contains(token)
                ) output.Push(token);
            else if (Tokenizer.knownPairs.ContainsKey(token)) operators.Push(token);
            else if (Tokenizer.knownPairs.ContainsValue(token))
            {
                while (operators.Count > 0 && !Tokenizer.knownPairs.ContainsKey(operators.Peek()))
                {
                    output.Push(operators.Pop());
                }
                operators.Pop();
            }
            else if (Tokenizer.knownFunctions.ContainsKey(token)) operators.Push(token);
            else // Operators
            {
                while (operators.Count > 0 && Tokenizer.precedence.ContainsKey(operators.Peek()) &&
                       Tokenizer.precedence[operators.Peek()] >= Tokenizer.precedence[token])
                {
                    output.Push(operators.Pop());
                }
                operators.Push(token);
            }
        }

        while (operators.Count > 0)
        {
            output.Push(operators.Pop());
        }

        return output.ToList();
    }

    public static double Evaluate(List<string> tokens, Stack<double> values, Stack<string> operators)
    {
        int pos = 0;
        while (pos < tokens.Count)
        {
            string token = tokens[pos];
            
            if (double.TryParse(token, out double number)) values.Push(number);
            else if (Tokenizer.knownPairs.ContainsKey(token)) operators.Push(token);
            else if (Tokenizer.knownPairs.ContainsValue(token))
            {
                while (!Tokenizer.knownPairs.ContainsKey(operators.Peek()))
                {
                    ProcessOperator(values, operators);
                }
                operators.Pop(); // Remove '('
            }
            else if (Tokenizer.knownFunctions.ContainsKey(token)) operators.Push(token);
            else if (Tokenizer.knownInfixOperators.Contains(token))
            {
                operators.Push(token);
                ProcessOperator(values, operators);
            }
            
            pos++;
        }

        while (operators.Count > 0)
        {
            ProcessOperator(values, operators);
        }

        return values.Pop();
    }


    private static void ProcessOperator(Stack<double> values, Stack<string> operators)
    {
        if (operators.Peek() == "root")
        {
            double exponent = values.Pop();
            double baseValue = values.Pop();
            values.Push(Math.Pow(baseValue, 1.0 / exponent));
            operators.Pop();
            return;
        }
        
        if (operators.Peek() == "pow")
        {
            double baseValue = values.Pop();
            double exponent = values.Pop();
            values.Push(Math.Pow(baseValue, exponent));
            operators.Pop();
            return;
        }

        double right = values.Pop();
        double left = values.Pop();
        string op = operators.Pop();

        switch (op.ToLower())
        {
            case "+": values.Push(left + right); break;
            case "-": values.Push(left - right); break;
            case "*": values.Push(left * right); break;
            case "/": values.Push(left / right); break;
            case "mod": values.Push(left % right); break;
            case "^": values.Push(Math.Pow(left, right)); break;
        }
    }
}