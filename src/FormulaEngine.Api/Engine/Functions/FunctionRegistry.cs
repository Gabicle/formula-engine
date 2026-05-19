using FormulaEngine.Api.Engine.Functions.Languages;

namespace FormulaEngine.Api.Engine.Functions;

public sealed class FunctionRegistry
{
    private const string UnknownFunctionMessage = "Unknown function: ";
    private const string MissingMappingsMessage = "language is missing canonical function mappings: ";
    private const string DuplicateAliasMessage = "Duplicate function alias found: ";

    private readonly Dictionary<string, string> _aliasToCanonical;

    public FunctionRegistry(IEnumerable<IFunctionLanguage> languages)
    {
        _aliasToCanonical = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var language in languages)
        {
            ValidateMappings(language);
            MergeMappings(language);
        }
    }

    public bool IsKnownFunction(string name) =>
        _aliasToCanonical.ContainsKey(name);

    public string Resolve(string name) =>
        _aliasToCanonical.TryGetValue(name, out var canonical)
            ? canonical
            : throw new InvalidOperationException(UnknownFunctionMessage + name);

    private static void ValidateMappings(IFunctionLanguage language)
    {
        var mappings = language.GetMappings();
        var canonicalValues = mappings.Values.ToHashSet();
        var missing = CanonicalFunctionNames.All
            .Where(c => !canonicalValues.Contains(c))
            .ToList();

        if (missing.Count > 0)
        {
            throw new InvalidOperationException(
                $"{language.GetType().Name} {MissingMappingsMessage}{string.Join(", ", missing)}");
        }
    }

    private void MergeMappings(IFunctionLanguage language)
    {
        foreach (var (alias, canonical) in language.GetMappings())
        {
            if (!_aliasToCanonical.TryAdd(alias, canonical))
            {
                throw new InvalidOperationException(DuplicateAliasMessage + alias);
            }
        }
    }
}
