using FormulaEngine.Api.Engine.Expressions;
using FormulaEngine.Api.Engine.Localization;
using System.Globalization;

namespace FormulaEngine.Api.Engine;

public class Parser
{

    private IReadOnlyList<Token> Tokens { get; init; }

    private readonly ILocaleSettings _localeSettings;


    private int Position { get; set; }
    public Parser(IReadOnlyList<Token> tokens,
     ILocaleSettings localeSettings)
    {
        Tokens = tokens;
        _localeSettings = localeSettings;
    }


    public Token Peek() => Tokens[Position];

    public Token Advance() => Tokens[Position++];

    public bool IsAtEnd() => Tokens[Position] == Token.EndOfExpression;


    public bool Check(TokenType type) => Peek().Type == type;

    public bool Match(TokenType type)
    {
        var isMatch = Check(type);
        if (isMatch)
        {
            Advance();
        }
        return isMatch;
    }


    public IExpression Parse() => ParseComparison();

    public IExpression ParsePrimary()
    {
        var currentToken = Peek();

        if (Match(TokenType.Number))
        {
            return GetNumberNode(currentToken, _localeSettings);
        }

        if (Match(TokenType.CellReference))
        {
            return GetCellReferenceNode(currentToken);
        }

        if (Match(TokenType.Function))
        {
            return GetFunctionNode(currentToken);
        }


        throw new NotImplementedException();
    }



    public IExpression ParseFactor()
    {
        var left = ParsePrimary();

        while (Check(TokenType.Multiply) || Check(TokenType.Divide))
        {
            var op = Match(TokenType.Multiply) ? BinaryOperator.Multiply : BinaryOperator.Divide;
            var right = ParsePrimary();
            left = new BinaryNode(left, right, op);
        }

        return left;
    }

    public IExpression ParseTerm()
    {
        var left = ParseFactor();

        while (Check(TokenType.Plus) || Check(TokenType.Minus))
        {
            var op = Match(TokenType.Plus) ? BinaryOperator.Add : BinaryOperator.Subtract;
            var right = ParseFactor();
            left = new BinaryNode(left, right, op);
        }

        return left;
    }

    public IExpression ParseComparison()
    {
        var left = ParseTerm();

        while (Check(TokenType.GreaterThan) || Check(TokenType.LessThan) || Check(TokenType.Equals))
        {
            BinaryOperator op;
            if (Match(TokenType.GreaterThan)) op = BinaryOperator.GreaterThan;
            else if (Match(TokenType.LessThan)) op = BinaryOperator.LessThan;
            else { Match(TokenType.Equals); op = BinaryOperator.Equals; }

            var right = ParseTerm();
            left = new BinaryNode(left, right, op);
        }

        return left;
    }

    private IExpression GetFunctionNode(Token currentToken)
    {
        var name = currentToken.Value;
        Match(TokenType.LeftParenthesis);

        var arguments = new List<IExpression>();

        while (!Check(TokenType.RightParenthesis))
        {
            arguments.Add(ParsePrimary());
            Match(TokenType.Comma);
        }

        Match(TokenType.RightParenthesis);
        return new FunctionNode(name, arguments);
    }

    private static CellReferenceNode GetCellReferenceNode(Token currentToken) => new(currentToken.Value);
    private static NumberNode GetNumberNode(Token currentToken, ILocaleSettings localeSettings)
    {
        var value = double.Parse(currentToken.Value, new CultureInfo(localeSettings.LocaleCode));
        return new NumberNode(value);
    }


}
