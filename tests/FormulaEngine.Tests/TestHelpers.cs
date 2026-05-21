using FormulaEngine.Api;
using FormulaEngine.Api.Engine;
using FormulaEngine.Api.Engine.Functions;
using FormulaEngine.Api.Engine.Functions.Languages;
using FormulaEngine.Api.Engine.Localization;
using Microsoft.Extensions.Localization;
using Moq;

namespace FormulaEngine.Tests;

public static class TestHelpers
{
    public static Lexer CreateLexer(ILocaleSettings? locale = null)
    {
        var mock = new Mock<IStringLocalizer<ErrorMessages>>();
        mock.Setup(m => m[It.IsAny<string>()]).Returns((string key) => new LocalizedString(key, key));

        var functionRegistry = new FunctionRegistry([new EnglishFunctions()]);

        return new Lexer(functionRegistry, locale ?? EnglishLocale(), mock.Object);
    }

    public static ILocaleSettings EnglishLocale()
    {
        var mock = new Mock<ILocaleSettings>();
        mock.Setup(m => m.DecimalSeparator).Returns('.');
        mock.Setup(m => m.ArgumentSeparator).Returns(',');
        mock.Setup(m => m.LocaleCode).Returns("en-GB");
        return mock.Object;
    }

    public static ILocaleSettings FrenchLocale()
    {
        var mock = new Mock<ILocaleSettings>();
        mock.Setup(m => m.DecimalSeparator).Returns(',');
        mock.Setup(m => m.ArgumentSeparator).Returns(';');
        mock.Setup(m => m.LocaleCode).Returns("fr-FR");
        return mock.Object;
    }
}
