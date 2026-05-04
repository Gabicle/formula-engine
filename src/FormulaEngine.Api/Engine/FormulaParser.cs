using System.Text.RegularExpressions;

namespace FormulaEngine.Api.Engine;

public partial class FormulaParser
{
    // Matches bracketed references like [Gross Profit] or plain words like Units
    [GeneratedRegex(@"\[([^\]]+)\]|\p{L}[\p{L}\p{N}_]*")]
    private static partial Regex CellKeyPatternRegex();

    private static readonly HashSet<string> ReservedWords = new(StringComparer.OrdinalIgnoreCase)
    {
        "true", "false", "and", "or", "not", "if", "else"
    };

    public static IReadOnlySet<string> ExtractCellReferences(string expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
            return new HashSet<string>();

        var references = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (Match match in CellKeyPatternRegex().Matches(expression))
        {
            // If group 1 captured something it was a bracketed reference
            // Use the inner value without the brackets
            var token = match.Groups[1].Success
                ? match.Groups[1].Value.Trim()
                : match.Value.Trim();

            if (!ReservedWords.Contains(token))
                references.Add(token);
        }

        return references;
    }
}