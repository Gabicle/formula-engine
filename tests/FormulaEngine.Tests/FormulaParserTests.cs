using FormulaEngine.Api.Engine;

namespace FormulaEngine.Tests;

public class FormulaParserTests
{
    [Fact]
    public void ExtractCellReferences_SimpleExpression_ReturnsCellKeys()
    {
        var result = FormulaParser.ExtractCellReferences("Units * Price");

        Assert.Equal(2, result.Count);
        Assert.Contains("Units", result);
        Assert.Contains("Price", result);
    }

    [Fact]
    public void ExtractCellReferences_ExpressionWithNumber_ReturnsOnlyCellKeys()
    {
        var result = FormulaParser.ExtractCellReferences("Units * 30");

        Assert.Single(result);
        Assert.Contains("Units", result);
    }

    [Fact]
    public void ExtractCellReferences_RawValue_ReturnsEmptySet()
    {
        var result = FormulaParser.ExtractCellReferences("1000");

        Assert.Empty(result);
    }

    [Fact]
    public void ExtractCellReferences_EmptyExpression_ReturnsEmptySet()
    {
        var result = FormulaParser.ExtractCellReferences("");

        Assert.Empty(result);
    }

    [Fact]
    public void ExtractCellReferences_ReservedWords_AreNotReturned()
    {
        var result = FormulaParser.ExtractCellReferences("if Units > 0");

        Assert.DoesNotContain("if", result);
        Assert.Contains("Units", result);
    }

    [Fact]
    public void ExtractCellReferences_IfElseExpression_ReservedWordsNotReturned()
    {
        var result = FormulaParser.ExtractCellReferences("if Units > 0 else Discount");

        Assert.DoesNotContain("if", result);
        Assert.DoesNotContain("else", result);
        Assert.Contains("Units", result);
        Assert.Contains("Discount", result);
    }

    [Fact]
    public void ExtractCellReferences_IsCaseInsensitive()
    {
        var result = FormulaParser.ExtractCellReferences("units * UNITS");

        Assert.Single(result);
    }

    [Fact]
    public void ExtractCellReferences_BracketedMultiWordKey_ReturnsSingleReference()
    {
        var result = FormulaParser.ExtractCellReferences("[Gross Profit] * 0.2");

        Assert.Single(result);
        Assert.Contains("Gross Profit", result);
    }

    [Fact]
    public void ExtractCellReferences_MultipleBracketedKeys_ReturnsAllReferences()
    {
        var result = FormulaParser.ExtractCellReferences("[Gross Profit] - [Operating Costs]");

        Assert.Equal(2, result.Count);
        Assert.Contains("Gross Profit", result);
        Assert.Contains("Operating Costs", result);
    }

    [Fact]
    public void ExtractCellReferences_UnicodeKey_ReturnsReference()
    {
        var result = FormulaParser.ExtractCellReferences("[収益] * 0.2");

        Assert.Single(result);
        Assert.Contains("収益", result);
    }
}
