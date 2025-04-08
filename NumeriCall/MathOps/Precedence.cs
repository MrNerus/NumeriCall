using System;

namespace NumeriCall.MathOps;

public enum OperatorPrecedence
{
    None = 0,
    Additive = 1,
    Multiplicative = 2,
    Exponent = 3,
    Primary = 4
}
public class Precedence
{
    public static OperatorPrecedence GetOperatorPrecedence(string op)
    {
        return op switch
        {
            "+" => OperatorPrecedence.Additive,
            "-" => OperatorPrecedence.Additive,
            "*" => OperatorPrecedence.Multiplicative,
            "/" => OperatorPrecedence.Multiplicative,
            "%" => OperatorPrecedence.Multiplicative,
            "^" => OperatorPrecedence.Exponent,
            "(" => OperatorPrecedence.Primary,
            "{" => OperatorPrecedence.Primary,
            "[" => OperatorPrecedence.Primary,
            ")" => OperatorPrecedence.None,
            "}" => OperatorPrecedence.None,
            "]" => OperatorPrecedence.None,
            _ => OperatorPrecedence.None
        };
    }

}
