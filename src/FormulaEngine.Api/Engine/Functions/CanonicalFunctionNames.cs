namespace FormulaEngine.Api.Engine.Functions;

public static class CanonicalFunctionNames
{
    public const string Sum = "Sum";
    public const string If = "If";
    public const string Min = "Min";
    public const string Max = "Max";
    public const string Average = "Average";

    public static readonly IReadOnlySet<string> All = new HashSet<string>
    {
        Sum, If, Min, Max, Average
    };
}
