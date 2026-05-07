using static FormulaEngine.Api.Engine.Functions.CanonicalFunctionNames;

namespace FormulaEngine.Api.Engine.Functions.Languages;

public class FrenchFunctions: IFunctionLanguage
{
    public IReadOnlyDictionary<string, string> GetMappings() => new Dictionary<string, string>
    {
        ["SOMME"]   = Sum,
        ["SI"]      = If,
        ["MIN"]     = Min,
        ["MAX"]     = Max,
        ["MOYENNE"] = Average,
    };
}