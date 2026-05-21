using FormulaEngine.Api.Engine;
using FormulaEngine.Api.Engine.Expressions;
using FormulaEngine.Api.Engine.Functions;
using FormulaEngine.Api.Engine.Localization;
using Moq;

namespace FormulaEngine.Tests;

public class ParserTests
{
    [Fact]
    public void Parse_Addition_ReturnsCorrectValue()
    {
        var tokens = new List<Token> { Token.Number(2), Token.Plus, Token.Number(3), Token.EndOfExpression };
        var result = ParseTokens(tokens);
        Assert.Equal(5.0, result.Evaluate(new Dictionary<string, double>()));
    }

    [Fact]
    public void Parse_Subtraction_ReturnsCorrectValue()
    {
        var tokens = new List<Token> { Token.Number(10), Token.Minus, Token.Number(3), Token.EndOfExpression };
        var result = ParseTokens(tokens);
        Assert.Equal(7.0, result.Evaluate(new Dictionary<string, double>()));
    }

    [Fact]
    public void Parse_Multiplication_ReturnsCorrectValue()
    {
        var tokens = new List<Token> { Token.Number(3), Token.Multiply, Token.Number(4), Token.EndOfExpression };
        var result = ParseTokens(tokens);
        Assert.Equal(12.0, result.Evaluate(new Dictionary<string, double>()));
    }

    [Fact]
    public void Parse_Division_ReturnsCorrectValue()
    {
        var tokens = new List<Token> { Token.Number(10), Token.Divide, Token.Number(2), Token.EndOfExpression };
        var result = ParseTokens(tokens);
        Assert.Equal(5.0, result.Evaluate(new Dictionary<string, double>()));
    }

    [Fact]
    public void Parse_PrecedenceMultiplicationOverAddition()
    {
        var tokens = new List<Token>
        {
            Token.Number(2),
            Token.Plus,
            Token.Number(3),
            Token.Multiply,
            Token.Number(4),
            Token.EndOfExpression
        };
        var result = ParseTokens(tokens);
        Assert.Equal(14.0, result.Evaluate(new Dictionary<string, double>()));
    }

    [Fact]
    public void Parse_CellReference_ReturnsValueFromContext()
    {
        var tokens = new List<Token> { Token.CellReference("Revenue"), Token.EndOfExpression };
        var context = new Dictionary<string, double> { ["Revenue"] = 1000.0 };
        var result = ParseTokens(tokens);
        Assert.Equal(1000.0, result.Evaluate(context));
    }

    [Fact]
    public void Parse_BinaryWithCellReference()
    {
        var tokens = new List<Token>
        {
            Token.CellReference("Revenue"), Token.Multiply, Token.Number(2), Token.EndOfExpression
        };
        var context = new Dictionary<string, double> { ["Revenue"] = 500.0 };
        var result = ParseTokens(tokens);
        Assert.Equal(1000.0, result.Evaluate(context));
    }

    [Fact]
    public void Parse_FunctionSum()
    {
        var tokens = new List<Token>
        {
            Token.Function(CanonicalFunctionNames.Sum),
            Token.LeftParenthesis,
            Token.CellReference("A"),
            Token.Comma,
            Token.CellReference("B"),
            Token.RightParenthesis,
            Token.EndOfExpression
        };
        var context = new Dictionary<string, double> { ["A"] = 3.0, ["B"] = 7.0 };
        var result = ParseTokens(tokens);
        Assert.Equal(10.0, result.Evaluate(context));
    }

    [Fact]
    public void Parse_FunctionIf_TrueBranch()
    {
        var tokens = new List<Token>
        {
            Token.Function(CanonicalFunctionNames.If),
            Token.LeftParenthesis,
            Token.CellReference("A"),
            Token.GreaterThan,
            Token.Number(0),
            Token.Comma,
            Token.CellReference("A"),
            Token.Comma,
            Token.Number(0),
            Token.RightParenthesis,
            Token.EndOfExpression
        };
        var context = new Dictionary<string, double> { ["A"] = 5.0 };
        var result = ParseTokens(tokens);
        Assert.Equal(5.0, result.Evaluate(context));
    }

    [Fact]
    public void Parse_FunctionIf_FalseBranch()
    {
        var tokens = new List<Token>
        {
            Token.Function(CanonicalFunctionNames.If),
            Token.LeftParenthesis,
            Token.CellReference("A"),
            Token.GreaterThan,
            Token.Number(0),
            Token.Comma,
            Token.CellReference("A"),
            Token.Comma,
            Token.Number(0),
            Token.RightParenthesis,
            Token.EndOfExpression
        };
        var context = new Dictionary<string, double> { ["A"] = -1.0 };
        var result = ParseTokens(tokens);
        Assert.Equal(0.0, result.Evaluate(context));
    }

    [Fact]
    public void Parse_GreaterThan_ReturnsOne()
    {
        var tokens = new List<Token> { Token.Number(5), Token.GreaterThan, Token.Number(3), Token.EndOfExpression };
        var result = ParseTokens(tokens);
        Assert.Equal(1.0, result.Evaluate(new Dictionary<string, double>()));
    }

    [Fact]
    public void Parse_GreaterThan_ReturnsZero()
    {
        var tokens = new List<Token> { Token.Number(3), Token.GreaterThan, Token.Number(5), Token.EndOfExpression };
        var result = ParseTokens(tokens);
        Assert.Equal(0.0, result.Evaluate(new Dictionary<string, double>()));
    }

    [Fact]
    public void Parse_UnknownCellReference_ThrowsInvalidOperationException()
    {
        var tokens = new List<Token> { Token.CellReference("Unknown"), Token.EndOfExpression };
        var result = ParseTokens(tokens);
        Assert.Throws<InvalidOperationException>(() => result.Evaluate(new Dictionary<string, double>()));
    }

    private static IExpression ParseTokens(IReadOnlyList<Token> tokens)
    {
        var parser = new Parser(tokens, TestHelpers.EnglishLocale());
        return parser.Parse();
    }

    private static IExpression Parse(string expression)
    {
        var lexer = TestHelpers.CreateLexer();

        var tokens = lexer.Tokenize(expression);

        var parser = new Parser(tokens, TestHelpers.EnglishLocale());

        return parser.Parse();
    }
}
