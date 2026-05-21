using FormulaEngine.Api.Engine;
using FormulaEngine.Api.Engine.Functions;
using FormulaEngine.Api.Engine.Localization;

namespace FormulaEngine.Tests;

public class LexerTests
{
    private static Lexer Create() => TestHelpers.CreateLexer();
    private static Lexer Create(ILocaleSettings localeSettings) => TestHelpers.CreateLexer(localeSettings);

    [Fact]
    public void Tokenize_EmptyExpression_ReturnsOnlyEndOfExpression()
    {
        var lexer = Create();
        var result = lexer.Tokenize("");
        Assert.Single(result);
        Assert.Equal(Token.EndOfExpression, result[0]);
    }

    [Fact]
    public void Tokenize_WhitespaceOnly_ReturnsOnlyEndOfExpression()
    {
        var lexer = Create();
        var result = lexer.Tokenize(" ");
        Assert.Single(result);
        Assert.Equal(Token.EndOfExpression, result[0]);
    }

    [Fact]
    public void Tokenize_BracketedCellReference_ReturnsCellReferenceToken()
    {
        var lexer = Create();
        var result = lexer.Tokenize("[Revenue]");
        Assert.Equal(Token.CellReference("Revenue"), result[0]);
    }

    [Fact]
    public void Tokenize_UnclosedBracket_ThrowsInvalidOperationException()
    {
        var lexer = Create();
        Assert.Throws<InvalidOperationException>(() => lexer.Tokenize("[Revenue"));
    }

    [Fact]
    public void Tokenize_KnownFunction_ReturnsFunctionToken()
    {
        var lexer = Create();
        var result = lexer.Tokenize("SUM([Revenue], [Cost]");
        Assert.Equal(Token.Function(CanonicalFunctionNames.Sum), result[0]);
    }

    [Fact]
    public void Tokenize_UnknownWord_ThrowsInvalidOperationException()
    {
        var lexer = Create();
        Assert.Throws<InvalidOperationException>(() => lexer.Tokenize("UNKNOWN([Revenue]"));
    }

    [Fact]
    public void Tokenize_Integer_ReturnsNumberToken()
    {
        var lexer = Create();
        var result = lexer.Tokenize("42");
        Assert.Equal(Token.Number(42), result[0]);
    }

    [Fact]
    public void Tokenize_Decimal_ReturnsNumberToken()
    {
        var lexer = Create();
        var result = lexer.Tokenize("3.14");
        Assert.Equal(Token.Number(3.14), result[0]);
    }

    [Fact]
    public void Tokenize_FrenchDecimal_ReturnsNumberToken()
    {
        var lexer = Create(TestHelpers.FrenchLocale());
        var result = lexer.Tokenize("3,14");
        Assert.Equal(Token.Number(3.14), result[0]);
    }

    [Fact]
    public void Tokenize_Operators_ReturnsCorrectTokens()
    {
        var lexer = Create();
        var result = lexer.Tokenize("1+2");
        Assert.Equal(4, result.Count);
        Assert.Equal(Token.Number(1), result[0]);
        Assert.Equal(Token.Plus, result[1]);
        Assert.Equal(Token.Number(2), result[2]);
        Assert.Equal(Token.EndOfExpression, result[3]);
    }

    [Fact]
    public void Tokenize_AlwaysEndsWithEndOfExpression()
    {
        var lexer = Create();
        var result = lexer.Tokenize("42");
        Assert.Equal(Token.EndOfExpression, result[1]);
    }
}
