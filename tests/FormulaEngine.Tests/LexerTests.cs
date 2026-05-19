using FormulaEngine.Api;
using FormulaEngine.Api.Engine;
using FormulaEngine.Api.Engine.Functions;
using FormulaEngine.Api.Engine.Functions.Languages;
using FormulaEngine.Api.Engine.Localization;
using Microsoft.Extensions.Localization;
using Moq;

namespace FormulaEngine.Tests;

public class LexerTests
{
    private static Lexer Create() => Create(EnglishLocale());

    private static Lexer Create(ILocaleSettings localeSettings)
    {
        var mock = new Mock<IStringLocalizer<ErrorMessages>>();
        mock.Setup(m => m[It.IsAny<string>()]).Returns((string key) => new LocalizedString(key, key));

        var functionRegistry = new FunctionRegistry([new EnglishFunctions()]);

        return new(functionRegistry, localeSettings, mock.Object);
    }

    private static ILocaleSettings EnglishLocale()
    {
        var mock = new Mock<ILocaleSettings>();
        mock.Setup(m => m.DecimalSeparator).Returns('.');
        mock.Setup(m => m.ArgumentSeparator).Returns(',');
        mock.Setup(m => m.LocaleCode).Returns("en-GB");
        return mock.Object;
    }

    private static ILocaleSettings FrenchLocale()
    {
        var mock = new Mock<ILocaleSettings>();
        mock.Setup(m => m.DecimalSeparator).Returns(',');
        mock.Setup(m => m.ArgumentSeparator).Returns(';');
        mock.Setup(m => m.LocaleCode).Returns("fr-FR");
        return mock.Object;
    }

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
        var lexer = Create(FrenchLocale());

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
