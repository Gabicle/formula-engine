using FormulaEngine.Api.Engine.Functions;
using FormulaEngine.Api.Engine.Localization;
using Microsoft.Extensions.Localization;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace FormulaEngine.Api.Engine;



public sealed class Lexer(
    FunctionRegistry functionRegistry,
    ILocaleSettings localeSettings,
    IStringLocalizer<ErrorMessages> localizer)
{
    private readonly FunctionRegistry _functionRegistry = functionRegistry;
    private readonly ILocaleSettings _localeSettings = localeSettings;
    private readonly IStringLocalizer<ErrorMessages> _localizer = localizer;

    public IReadOnlyList<Token> Tokenize(string expression)
    {
        var tokens = new List<Token>();
        var span = expression.AsSpan();
        var pos = 0;

        while (pos < span.Length)
        {
            if (IsWhitespace(span[pos]))
            {
                pos++;
                continue;
            }

            var token = ReadNextToken(span, ref pos);
            tokens.Add(token);
        }

        tokens.Add(Token.EndOfExpression);
        return tokens;
    }

    private Token ReadNextToken(ReadOnlySpan<char> span, ref int pos)
    {
        var c = span[pos];

        if (c == '[')
        {
            return ReadBracketedCellReference(span, ref pos);
        }

        if (char.IsLetter(c))
        {
            return ReadWord(span, ref pos);
        }

        if (char.IsDigit(c))
        {
            return ReadNumber(span, ref pos);
        }

        if (c == _localeSettings.ArgumentSeparator)
        { pos++; return Token.Comma; }
        if (c == '+')
        { pos++; return Token.Plus; }
        if (c == '-')
        { pos++; return Token.Minus; }
        if (c == '*')
        { pos++; return Token.Multiply; }
        if (c == '/')
        { pos++; return Token.Divide; }
        if (c == '(')
        { pos++; return Token.LeftParenthesis; }
        if (c == ')')
        { pos++; return Token.RightParenthesis; }
        if (c == '>')
        { pos++; return Token.GreaterThan; }
        if (c == '<')
        { pos++; return Token.LessThan; }
        if (c == '=')
        { pos++; return Token.EqualSign; }

        throw new InvalidOperationException(
            string.Format(_localizer["UnexpectedCharacter"], c, pos));
    }

    private Token ReadBracketedCellReference(ReadOnlySpan<char> span, ref int pos)
    {
        pos++;
        var start = pos;
        var hasClosingBracket = false;

        while (pos < span.Length)
        {
            if (span[pos] == ']')
            {
                hasClosingBracket = true;
                break;
            }
            pos++;
        }

        if (!hasClosingBracket)
        {
            throw new InvalidOperationException(
                string.Format(_localizer["UnclosedBracket"], pos));
        }

        var raw = span[start..pos];
        pos++;
        return Token.CellReference(raw.ToString());
    }

    private Token ReadWord(ReadOnlySpan<char> span, ref int pos)
    {
        var start = pos;

        while (pos < span.Length)
        {
            if (!char.IsLetter(span[pos]))
            {
                break;
            }

            pos++;
        }

        var raw = span[start..pos];
        var word = raw.ToString();


        if (!_functionRegistry.IsKnownFunction(word))
        {
            throw new InvalidOperationException(
              string.Format(_localizer["UnknownFunction"], pos));
        }

        return Token.Function(_functionRegistry.Resolve(word));
    }

    private Token ReadNumber(ReadOnlySpan<char> span, ref int pos)
    {
        var start = pos;
        var hasDecimal = false;

        while (pos < span.Length)
        {
            if (char.IsDigit(span[pos]))
            {
                pos++;
                continue;
            }

            if (!hasDecimal && span[pos] == _localeSettings.DecimalSeparator)
            {
                hasDecimal = true;
                pos++;
                continue;
            }

            break;
        }

        var raw = span[start..pos];
        var value = double.Parse(raw, new CultureInfo(_localeSettings.LocaleCode));
        return Token.Number(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsWhitespace(char c) => c is ' ' or '\t';
}
