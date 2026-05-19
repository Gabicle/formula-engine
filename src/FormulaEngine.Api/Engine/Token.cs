namespace FormulaEngine.Api.Engine;

public record Token(TokenType Type, string Value)
{
    public static readonly Token Plus = new(TokenType.Plus, "+");
    public static readonly Token Minus = new(TokenType.Minus, "-");
    public static readonly Token Multiply = new(TokenType.Multiply, "*");
    public static readonly Token Divide = new(TokenType.Divide, "/");
    public static readonly Token GreaterThan = new(TokenType.GreaterThan, ">");
    public static readonly Token LessThan = new(TokenType.LessThan, "<");
    public static readonly Token EqualSign = new(TokenType.Equals, "="); public static readonly Token LeftParenthesis = new(TokenType.LeftParenthesis, "(");
    public static readonly Token RightParenthesis = new(TokenType.RightParenthesis, ")");
    public static readonly Token Comma = new(TokenType.Comma, ",");
    public static readonly Token EndOfExpression = new(TokenType.EndOfExpression, "");

    public static Token Number(double num) => new(TokenType.Number, num.ToString());

    public static Token CellReference(string val) => new(TokenType.CellReference, val);
    public static Token Function(string v) => new(TokenType.Function, v);
}
