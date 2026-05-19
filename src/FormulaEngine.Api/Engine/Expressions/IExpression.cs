namespace FormulaEngine.Api.Engine.Expressions;


public interface IExpression
{
    double Evaluate(IReadOnlyDictionary<string, double> context);
}
