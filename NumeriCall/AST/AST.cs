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
        try {
            var paramsEvaluated = _parameters.Select(p => p.Evaluate()).ToList();
            
            return _name switch
            {
                "abs" => Math.Abs(paramsEvaluated[0]),
                "root" => Math.Pow(paramsEvaluated[1], 1.0 / paramsEvaluated[0]),
                "round" => Math.Round(paramsEvaluated[0],MidpointRounding.AwayFromZero),
                "celi" => Math.Ceiling(paramsEvaluated[0]),
                "floor" => Math.Floor(paramsEvaluated[0]),
                "mod" => paramsEvaluated[0] % paramsEvaluated[1],
                "pow" => Math.Pow(paramsEvaluated[0], paramsEvaluated[1]),
                _ => throw new ArgumentException($"Unknown function: {_name}")
            };

        } catch (ArgumentOutOfRangeException e) {
            throw new Exception($"Syntax error: Insufficient arguments for function {_name}.");
        }
    }
}

public class UnaryOperationNode(IAstNode operand, string op) : IAstNode
{
    private readonly IAstNode _operand = operand;
    private readonly string _operator = op;

    public double Evaluate()
    {
        var operandValue = _operand.Evaluate();

        return _operator switch
        {
            "-" => -operandValue,
            "+" => operandValue,
            _ => throw new ArgumentException($"Unknown unary operator: {_operator}")
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
        while (_position < _tokens.Count)
        {
            var currentPrecedence = Precedence.GetOperatorPrecedence(_tokens[_position]);
            if (currentPrecedence < OperatorPrecedence.Additive)
                break;
                
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
        while (_position < _tokens.Count)
        {
            var currentPrecedence = Precedence.GetOperatorPrecedence(_tokens[_position]);
            if (currentPrecedence == OperatorPrecedence.Multiplicative)
            {
                var op = _tokens[_position];
                _position++;
                var right = ParseFactor();
                left = new BinaryOperationNode(left, right, op);
            }
            else
            {
                break;
            }
        }
        return left;
    }

    private IAstNode ParseFactor()
    {
        var left = ParsePrimary();
        while (_position < _tokens.Count)
        {
            var currentPrecedence = Precedence.GetOperatorPrecedence(_tokens[_position]);
            if (currentPrecedence == OperatorPrecedence.Exponent)
            {
                var op = _tokens[_position];
                _position++;
                var right = ParsePrimary();
                left = new BinaryOperationNode(left, right, op);
            }
            else
            {
                break;
            }
        }
        return left;
    }

    private IAstNode ParsePrimary()
    {
        try {
            if (TokenService.UrinaryOperators.Contains(_tokens[_position]))
            {
                _position++;
                var operand = ParsePrimary(); // Recursively parse the operand after the unary '-'
                return new UnaryOperationNode(operand, "-");
            }
            
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

            if (TokenService.Constants.Contains(_tokens[_position]))
            {
                var constant = _tokens[_position];
                _position++;
                return new NumberNode(TokenService.GetConstantValue(constant));
            }

            if (!double.TryParse(_tokens[_position], out var value))
            {
                throw new ArgumentException($"Expected number at position {_position}");
            }

            _position++;
            return new NumberNode(value);

        } 
        catch (ArgumentOutOfRangeException e) {
            throw new Exception($"Syntax error near position {_position}. Incomplete Expression.");
        }
        catch (ArgumentException e) {
            throw new Exception($"Syntax error near position {_position}. {e.Message}.");
        }
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