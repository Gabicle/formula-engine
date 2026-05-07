namespace FormulaEngine.Api.Engine;

public enum TokenType
{
    Number,
    CellReference,
    Function,
    Plus,
    Minus,
    Multiply,
    Divide,
    GreaterThan,
    LessThan,
    Equals,
    LeftParenthesis,
    RightParenthesis,
    Comma,
    EndOfExpression
}
