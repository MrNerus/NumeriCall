using System;
using NumeriCall.MathOps;

namespace NumeriCall.AST;


public interface IAstNode
{
    public double Evaluate();
}

public class NumberNode(double value) : IAstNode
{
    private readonly double _value = value;
    public double Evaluate()
    {
        return _value;
    }
}

public class BinaryOperationNode(IAstNode left, IAstNode right, string op) : IAstNode
{
    protected readonly IAstNode _left = left;
    protected readonly IAstNode _right = right;
    protected readonly string _operator = op;
    protected readonly OperatorPrecedence _precedence = Precedence.GetOperatorPrecedence(op);


    public double Evaluate()
    {
        var leftVal = _left.Evaluate();
        var rightVal = _right.Evaluate();

        return _operator switch
        {
            "+" => leftVal + rightVal,
            "-" => leftVal - rightVal,
            "*" => leftVal * rightVal,
            "/" => leftVal / rightVal,
            "^" => Math.Pow(leftVal, rightVal),
            "%" => leftVal / rightVal * 100,
            "MOD" => leftVal % rightVal,
            _ => throw new ArgumentException($"Unknown operator: {_operator}")
        };
    }
}

public class FunctionNode(string name, List<IAstNode> parameters) : IAstNode
{
    private readonly string _name = name;
    private readonly List<IAstNode> _parameters = parameters;

    public double Evaluate()
    {
        var paramsEvaluated = _parameters.Select(p => p.Evaluate()).ToList();
        
        return _name switch
        {
            "abs" => Math.Abs(paramsEvaluated[0]),
            "root" => Math.Pow(paramsEvaluated[1], 1.0 / paramsEvaluated[0]),
            "round" => Math.Round(paramsEvaluated[1]),
            "celi" => Math.Ceiling(paramsEvaluated[1]),
            "floor" => Math.Floor(paramsEvaluated[1]),
            _ => throw new ArgumentException($"Unknown function: {_name}")
        };
    }
}

public class ExpressionParser(List<string> tokens)
{
    private readonly List<string> _tokens = tokens;
    private int _position = 0;

    public IAstNode Parse()
    {
        return ParseExpression();
    }

    private IAstNode ParseExpression()
    {
        var left = ParseTerm();

        while (_position < _tokens.Count && Precedence.GetOperatorPrecedence(_tokens[_position]) == OperatorPrecedence.Additive)
        {
            var op = _tokens[_position];
            _position++;
            var right = ParseTerm();
            
            left = new BinaryOperationNode(left, right, op);
        }

        return left;
    }

    private IAstNode ParseTerm()
    {
        var left = ParseFactor();

        while (_position < _tokens.Count && Precedence.GetOperatorPrecedence(_tokens[_position]) == OperatorPrecedence.Multiplicative)
        {
            var op = _tokens[_position];
            _position++;
            var right = ParseFactor();
            
            left = new BinaryOperationNode(left, right, op);
        }

        return left;
    }

    private IAstNode ParseFactor()
    {
        if (TokenService.Functions.Contains(_tokens[_position]))
        {
            var funcName = _tokens[_position];
            _position++; 
            
            if (!TokenService.Pairs.ContainsKey(_tokens[_position]))
                throw new ArgumentException($"Expected bracket after function name '{funcName}'");
            _position++; 
            
            var parameters = new List<IAstNode>();
            while (!TokenService.Pairs.ContainsValue(_tokens[_position]))
            {
                parameters.Add(ParseExpression());
                if (_tokens[_position] == ",")
                    _position++;
            }
            _position++;
            
            return new FunctionNode(funcName, parameters);
        }
        if (TokenService.Pairs.ContainsKey(_tokens[_position]))
        {
            _position++; 
            var expr = ParseExpression();
            if (!TokenService.Pairs.ContainsValue(_tokens[_position]))
                throw new ArgumentException("Missing closing parenthesis");
            _position++; 
            return expr;
        }

        if (!double.TryParse(_tokens[_position], out var value))
        {
            throw new ArgumentException($"Expected number at position {_position}");
        }

        _position++;
        return new NumberNode(value);
    }
}

public class ExpressionEvaluator
{
    public static double Evaluate(List<string> expression)
    {
        var parser = new ExpressionParser(expression);
        var ast = parser.Parse();
        return ast.Evaluate();
    }
}