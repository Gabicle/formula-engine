namespace FormulaEngine.Api.Engine.Expressions;

public class BinaryNode : IExpression
{

    public BinaryNode(IExpression left, IExpression right, BinaryOperator op)
    {
        Left = left;
        Right = right;
        Op = op;
    }
    public IExpression Left { get; init; }
    public IExpression Right { get; init; }

    public BinaryOperator Op { get; init; }

    public double Evaluate(IReadOnlyDictionary<string, double> context)
    {
        var left = Left.Evaluate(context);
        var right = Right.Evaluate(context);

        return Op switch
        {
            BinaryOperator.Add => left + right,
            BinaryOperator.Subtract => left - right,
            BinaryOperator.Multiply => left * right,
            BinaryOperator.Divide => left / right,
            BinaryOperator.GreaterThan => left > right ? 1.0 : 0.0,
            BinaryOperator.LessThan => left < right ? 1.0 : 0.0,
            BinaryOperator.Equals => left == right ? 1.0 : 0.0,
            _ => throw new InvalidOperationException("Operator Not Implemented")
        };
    }
}
