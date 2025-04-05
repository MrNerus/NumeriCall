using System;
using System.Text.RegularExpressions;

namespace NumeriCall.MathOps;

public partial class TokenService
{
    public static readonly List<string> Tokens = ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9", ".", "+", "-", "*", "/", "(", ")", "^", "root", "pow", "mod", ",", "PI", "TAU"];

    public static readonly Dictionary<string, string> Pairs = new()
    {
        {"(", ")"},
    };

    public static readonly List<string> Functions = ["root", "pow"];

    public static readonly List<string> Numbers = ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "."];
    public static readonly List<string> Constants = ["PI", "TAU"];
    // public static readonly char separator = '␟';
    public static readonly char separator = '|';


    public static double GetConstantValue(string constant) {
        return constant.ToUpper() switch
        {
            "PI" => Math.PI,
            "TAU" => Math.Tau,
            _ => throw new ArgumentException($"Unknown constant: {constant}")
        };
    }

    public static List<string> Tokenize(string input)
    {
        var sortedTokens = Tokens.OrderByDescending(t => t.Length).ToList();
        var pattern = string.Join(separator, sortedTokens.Select(token => Regex.Escape(token)));
        var matches = Regex.Matches(input, pattern);

        List<string> charToken = [.. matches.Cast<Match>().Select(m => m.Value)];
        List<string> tokens = [];
        
        string currentToken = "";
        foreach (string token in charToken) 
        {
            if (Numbers.Contains(token)) 
            {
                currentToken += token;
            } 
            else 
            {
                if (!string.IsNullOrEmpty(currentToken)) 
                {
                    if (!NumberRegex().IsMatch(currentToken)) throw new ArgumentException($"Invalid number: {currentToken}");
                    tokens.Add(currentToken);
                    currentToken = "";
                }
                tokens.Add(token);
            }
        }
        if (!string.IsNullOrEmpty(currentToken)) 
        {
            tokens.Add(currentToken);
            currentToken = "";
        }
        return tokens;
    }

    [GeneratedRegex(@"^-?\d+(\.\d+)?$")]
    public static partial Regex NumberRegex();

}
