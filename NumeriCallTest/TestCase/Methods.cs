using System;
using NumeriCall;
using NumeriCall.AST;
using NumeriCall.MathOps;

namespace NumeriCallTest.TestCase;

public class Methods
{
    private readonly List<string> allTestCases = ["TestCase_Tokenizer", "TestCase_AST"];
    

    public bool TestChoice() {
        Console.WriteLine($"{string.Join("\n", allTestCases)}");
        Console.WriteLine("What to test [Comma Seperated Query / All]?");
        string toTestQry = (Console.ReadLine() ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(toTestQry)) return false;
        if (toTestQry.Equals("all", StringComparison.OrdinalIgnoreCase)) return TestSummary(allTestCases);
        
        List<string> listOfTests = [.. toTestQry.Split(',')];

        return TestSummary(listOfTests);
    }

    private bool TestSummary(List<string> testCases) {
        int testCounter = 0;
        int passCounter = 0;
        int failCounter = 0;

        foreach (string toTest in testCases)
        {
            bool tc_status = TestOne(toTest);
            if (tc_status) passCounter++;
            else failCounter++;
            testCounter++;
        }

        Console.WriteLine($"Test Summary: For {testCounter} tests performed, {passCounter} passed and {failCounter} failed."); 
        return failCounter == 0;
    }

    private bool TestOne(string testCase)
    {
        bool tc_status = false;
        switch (testCase)
        {
            case "TestCase_Tokenizer":
                tc_status = TestCase_Tokenizer();
                break;
            case "TestCase_AST":
                tc_status = TestCase_AST();
                break;
        }

        Console.WriteLine($"{testCase}: {(tc_status ? "PASS" : "FAIL")}");
        return tc_status;
    }

    private static bool IsEqual<T>(List<T> left, List<T> right)
    {
        int leftCount = left.Count;
        int rightCount = right.Count;
        if (leftCount != rightCount) return false;
        for (int i = 0; i < leftCount; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(left[i], right[i])) return false;
        }
        return true;
    
    }
    private bool TestCase_Tokenizer()
    {
        Dictionary<string, List<string>> testCases = new()
        {
            {"5 + 5 + 5", ["5", "+", "5", "+", "5"]},
            {"5 5 5", ["555"]},
            {"1 2 3", ["123"]},
            {"1 2 1", ["121"]},
            {"5 + 10 + root(2, 16)", ["5", "+", "10", "+", "root", "(", "2", ",", "16", ")"]},
            {"root(2, root(3, 64)) + root(2, root(5, 1024))", ["root", "(", "2", ",", "root", "(", "3", ",", "64", ")", ")", "+", "root", "(", "2", ",", "root", "(", "5", ",", "1024", ")", ")"]},
        };

        bool tc_status = true;

        foreach (var testCase in testCases)
        {
            var tokens = TokenService.Tokenize(testCase.Key);
            if (!IsEqual(tokens, testCase.Value)) {
                Console.WriteLine($"TEST FAIL ::: TestCase_Tokenizer ::: Input::{testCase.Key} ::: Result::[{string.Join("", tokens)}] ::: ExpectedReslt::[{string.Join("", testCase.Value)}]");
                tc_status = false;
            } 
            else 
            {
                Console.WriteLine($"TEST PASS ::: TestCase_Tokenizer ::: Input::{testCase.Key} ::: Result::[{string.Join("", tokens)}] ::: ExpectedReslt::[{string.Join("", testCase.Value)}]");
            };            
        }
        return tc_status;
    }

    private bool TestCase_AST() {
        Dictionary<string, double> testCases = new()
        {
            {"1 + 2 * 3", 7},
            {"(1 + 2) * 3", 9},
            {"root(2, 16)", 4},
            {"root(2, root(3, 64))", 2},
            {"5 + 10 + root ( 2 , 16 )", 19},
            {"root(2, root(3, 64)) + root(2, root(5, 1024))", 4},
            {"1 + 2 - 3 + 4", 4},
            {"1 + 2 * 3 ^ 2", 19},
            {"(1 + 2 * 3) ^ 2", 49},
            {"root(2, 9) + root(3, 27)", 6},
            {"pow(2, 3)", 8},
            {"pow(2, pow(2, 2))", 16},
            {"mod(10, 3)", 1},
            {"mod(10 + 5, 6)", 3},
            {"abs(-5)", 5},
            {"abs(-root(2, 16))", 4},
            {"celi(4.2)", 5},
            {"floor(4.9)", 4},
            {"round(4.4)", 4},
            {"round(4.5)", 5},
            {"PI * 2", Math.PI * 2},
            {"TAU / 2", Math.PI},
            {"root(2, 16) + pow(2, 2) * mod(10, 3)", 8},
            {"pow(2, 3 + 1)", 16},
            {"pow(2, root(2, 16))", 16},
            {"round(root(2, 15))", 4},
            {"round(root(2, 16) + 0.4)", 4},
            {"abs(-PI) + abs(-TAU)", Math.PI + 2 * Math.PI},
            {"- - 1", 1},
            {"- 1 - 2", -3},
            {"- 1 - - 2", 1},
            {"- 1 + + 2", 1},
            {"- 1 - + 2", -3},
            {"- 1 + - 2", -3},
            {"- - -(- 1 - - - 2)", 3},
            {"- - - -(- 1 - - - 2)", -3},
        };
        

        bool tc_status = true;

        foreach (var testCase in testCases)
        {
            double result = ExpressionEvaluator.Evaluate(TokenService.Tokenize(testCase.Key));
            if (result != testCase.Value) {
                Console.WriteLine($"TEST FAIL ::: TestCase_AST ::: Input::{testCase.Key} ::: Result::{result} ::: ExpectedReslt::{testCase.Value}");
                tc_status = false;
            }
            else {
                Console.WriteLine($"TEST PASS ::: TestCase_AST ::: Input::{testCase.Key} ::: Result::{result} ::: ExpectedReslt::{testCase.Value}");
            }            
        }


        Dictionary<string, string> testCases_error = new()
        {
            {"1 +", "Syntax error"},
            {"* 3", "Syntax error"},
            {"(1 + 2", "Syntax error"},
            {"1 + 2)", "Syntax error"},
            {"root(2)", "Syntax error"},
            {"root(2, 16, 8)", "Syntax error"},
            {"pow()", "Syntax error"},
            {"abs(1, 2)", "Syntax error"},
            {"mod(10)", "Syntax error"},
            {"PI()", "Syntax error"},
            {"TAU(2)", "Syntax error"},
            {"round(,4)", "Syntax error"},
            {"1 + + 2", "Syntax error"},
            {"1 2", "Syntax error"},
            {"root(2 root(2, 16))", "Syntax error"},
            {"pow(2, )", "Syntax error"},
            {"mod(,3)", "Syntax error"},
            {",1 + 2", "Syntax error"},
        };
        
        foreach (var testCase in testCases_error)
        {
            try {
                double result = ExpressionEvaluator.Evaluate(TokenService.Tokenize(testCase.Key));
            } 
            catch (Exception e)
            {
                if (e.Message.StartsWith(testCase.Value)) {
                    Console.WriteLine($"ERR TEST PASS ::: TestCase_AST ::: Input::{testCase.Key} ::: Result::{e.Message} ::: ExpectedReslt::{testCase.Value}...");
                }
                else {
                    Console.WriteLine($"ERR TEST FAIL ::: TestCase_AST ::: Input::{testCase.Key} ::: Result::{e.Message} ::: ExpectedReslt::{testCase.Value}...");
                    tc_status = false;
                }            
            }
        }


        return tc_status;
    }

}
