namespace FormulaEngine.Api.Engine.Expressions;

public class CellReferenceNode : IExpression
{
    public CellReferenceNode(string value) => Reference = value;

    private string Reference { get; init; }
    public double Evaluate(IReadOnlyDictionary<string, double> context)
    {
        if (!context.TryGetValue(Reference, out var value))
        {
            throw new InvalidOperationException($"Cell reference '{Reference}' not found in context");
        }

        return value;
    }
}
