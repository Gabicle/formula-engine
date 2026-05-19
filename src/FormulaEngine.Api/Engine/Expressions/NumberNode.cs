namespace FormulaEngine.Api.Engine.Expressions;

public class NumberNode : IExpression
{
    public NumberNode(double num) => Num = num;

    private double Num { get; init; }


    public double Evaluate(IReadOnlyDictionary<string, double> context) => Num;
}
