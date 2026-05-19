using FormulaEngine.Api.Engine.Functions;

namespace FormulaEngine.Api.Engine.Expressions;

public class FunctionNode : IExpression
{
    public FunctionNode(string func, IReadOnlyList<IExpression> expressions)
    {
        Func = func;
        Expressions = expressions;
    }

    private string Func { get; init; }

    private IReadOnlyList<IExpression> Expressions { get; init; }


    public double Evaluate(IReadOnlyDictionary<string, double> context)
    {
        List<double> values = [];

        foreach (var expr in Expressions)
        {
            var result = expr.Evaluate(context);
            values.Add(result);
        }

        return Func switch
        {
            CanonicalFunctionNames.Sum => GetSumAggregate(values),
            CanonicalFunctionNames.Min => GetMin(values),
            CanonicalFunctionNames.Max => GetMax(values),
            CanonicalFunctionNames.Average => GetAverage(values),
            CanonicalFunctionNames.If => values[0] != 0 ? values[1] : values[2],
            _ => throw new InvalidOperationException("Function Not Implemented")

        };
    }

    private static double GetAverage(List<double> values) => values.Average();
    private static double GetMax(List<double> values) => values.Max();
    private static double GetMin(List<double> values) => values.Min();

    private static double GetSumAggregate(List<double> values) => values.Sum();
}
