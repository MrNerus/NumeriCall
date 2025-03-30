using System;
using System.Text.RegularExpressions;

namespace NumeriCall.Logics;

public partial class Tokenizer
{
    public static readonly List<string> knownTokens = ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9", ".", "+", "-", "*", "/", "(", ")", "^", "root", "pow", "mod", ",", "PI"];

    public static readonly Dictionary<string, string> knownPairs = new()
    {
        {"(", ")"},
    };
    public static readonly Dictionary<string, int> precedence = new() {
        {"+", 1},
        {"-", 1},
        {"*", 2},
        {"/", 2},
        {"mod", 2},
        {"^", 3},
        {"root", 4},
        {"pow", 4}
};
    public static readonly Dictionary<string, int> knownFunctions = new() {
        {"root", 2},
        {"pow", 2}
    };

    public static readonly List<string> knownInfixOperators = ["+", "-", "*", "/", "^", "mod"];
    public static readonly List<string> knownNumbers = ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "."];
    public static readonly List<string> knownConstants = ["PI"];
    public static readonly char separator = '|';

    public static List<string> Tokenize(string input)
    {
        var sortedTokens = knownTokens.OrderByDescending(t => t.Length).ToList();
        var pattern = string.Join(separator, sortedTokens.Select(token => Regex.Escape(token)));
        var matches = Regex.Matches(input, pattern);

        List<string> charToken = [.. matches.Cast<Match>().Select(m => m.Value)];
        List<string> tokens = [];
        
        string currentToken = "";
        foreach (string token in charToken) 
        {
            if (knownNumbers.Contains(token)) 
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
