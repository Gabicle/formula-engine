using FormulaEngine.Api.Engine.Expressions;
using FormulaEngine.Api.Engine.Localization;
using System.Globalization;

namespace FormulaEngine.Api.Engine;

public class Parser
{
    private readonly ILocaleSettings _localeSettings;
    private IReadOnlyList<Token> Tokens { get; init; }
    private int Position { get; set; }

    public Parser(IReadOnlyList<Token> tokens, ILocaleSettings localeSettings)
    {
        Tokens = tokens;
        _localeSettings = localeSettings;
    }

    public IExpression Parse() => ParseComparison();

    private Token Peek() => Tokens[Position];
    private Token Advance() => Tokens[Position++];
    private bool Check(TokenType type) => Peek().Type == type;

    private bool Match(TokenType type)
    {
        var isMatch = Check(type);
        if (isMatch)
        {
            Advance();
        }

        return isMatch;
    }

    private void Consume(TokenType type, string message)
    {
        if (!Check(type))
        {
            throw new InvalidOperationException(message);
        }

        Advance();
    }

    private IExpression ParseComparison()
    {
        var left = ParseTerm();

        while (Check(TokenType.GreaterThan) || Check(TokenType.LessThan) || Check(TokenType.Equals))
        {
            BinaryOperator op;
            if (Match(TokenType.GreaterThan))
            {
                op = BinaryOperator.GreaterThan;
            }
            else if (Match(TokenType.LessThan))
            {
                op = BinaryOperator.LessThan;
            }
            else
            {
                Match(TokenType.Equals); op = BinaryOperator.Equals;
            }

            var right = ParseTerm();
            left = new BinaryNode(left, right, op);
        }

        return left;
    }

    private IExpression ParseTerm()
    {
        var left = ParseFactor();

        while (Check(TokenType.Plus) || Check(TokenType.Minus))
        {
            BinaryOperator op;
            if (Match(TokenType.Plus))
            {
                op = BinaryOperator.Add;
            }
            else
            {
                Match(TokenType.Minus);
                op = BinaryOperator.Subtract;
            }
            var right = ParseFactor();

            left = new BinaryNode(left, right, op);
        }

        return left;
    }

    private IExpression ParseFactor()
    {
        var left = ParsePrimary();

        while (Check(TokenType.Multiply) || Check(TokenType.Divide))
        {
            BinaryOperator op;
            if (Match(TokenType.Multiply))
            {
                op = BinaryOperator.Multiply;
            }
            else
            {
                Match(TokenType.Divide);
                op = BinaryOperator.Divide;
            }
            var right = ParsePrimary();

            left = new BinaryNode(left, right, op);
        }

        return left;
    }

    private IExpression ParsePrimary()
    {
        var currentToken = Peek();

        if (Match(TokenType.Number))
            return GetNumberNode(currentToken, _localeSettings);

        if (Match(TokenType.CellReference))
            return GetCellReferenceNode(currentToken);

        if (Match(TokenType.Function))
            return GetFunctionNode(currentToken);

        throw new InvalidOperationException($"Unexpected token '{Peek().Value}' at position {Position}");
    }

    private FunctionNode GetFunctionNode(Token currentToken)
    {
        var name = currentToken.Value;
        Consume(TokenType.LeftParenthesis, $"Expected '(' after function '{name}'");

        var arguments = new List<IExpression>();

        while (!Check(TokenType.RightParenthesis))
        {
            arguments.Add(ParseComparison());
            Match(TokenType.Comma);
        }

        Consume(TokenType.RightParenthesis, $"Expected ')' after arguments in function '{name}'");
        return new FunctionNode(name, arguments);
    }

    private static CellReferenceNode GetCellReferenceNode(Token currentToken) =>
        new(currentToken.Value);

    private static NumberNode GetNumberNode(Token currentToken, ILocaleSettings localeSettings)
    {
        var value = double.Parse(currentToken.Value, new CultureInfo(localeSettings.LocaleCode));
        return new NumberNode(value);
    }
}
