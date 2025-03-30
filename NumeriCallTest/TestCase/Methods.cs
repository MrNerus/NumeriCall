using System;
using NumeriCall;
using NumeriCall.Logics;

namespace NumeriCallTest.TestCase;

public class Methods
{
    private readonly List<string> allTestCases = ["TestCase_Tokenizer"];
    

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
        };

        bool tc_status = true;

        foreach (var testCase in testCases)
        {
            var tokens = Tokenizer.Tokenize(testCase.Key);
            if (!IsEqual(tokens, testCase.Value)) {
                Console.WriteLine($"TEST FAIL:::TestCase_Tokenizer:::Input::{testCase.Key}:::Result::[{string.Join("", tokens)}]:::ExpectedReslt::[{string.Join("", testCase.Value)}]");
                tc_status = false;
            };            
        }
        return tc_status;
    }

}
