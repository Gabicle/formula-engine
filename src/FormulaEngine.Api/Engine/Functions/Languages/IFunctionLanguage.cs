namespace FormulaEngine.Api.Engine.Functions.Languages;

public interface IFunctionLanguage
{
    IReadOnlyDictionary<string, string> GetMappings();
}