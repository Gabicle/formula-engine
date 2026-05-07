using static FormulaEngine.Api.Engine.Functions.CanonicalFunctionNames;

namespace FormulaEngine.Api.Engine.Functions.Languages;

public class EnglishFunctions : IFunctionLanguage
{
    public IReadOnlyDictionary<string, string> GetMappings() => new Dictionary<string, string>
    {
        ["SUM"]     = Sum,
        ["IF"]      = If,
        ["MIN"]     = Min,
        ["MAX"]     = Max,
        ["AVERAGE"] = Average,
    };
}