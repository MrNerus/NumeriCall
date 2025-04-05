using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using NumeriCall.AST;

namespace NumeriCall.MathOps;

public class Calculator()
{
    public static double Calculate(string input)
    {
        double result = ExpressionEvaluator.Evaluate(TokenService.Tokenize(input));
        return result;
    }
}