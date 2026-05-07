namespace FormulaEngine.Api.Engine;

public class CellNode
{
    public Guid Id { get; init; }

    public Guid WorkspaceId { get; init; }

    public string Key { get; init; } = null!;

    public decimal Value { get; set; }

    public string? Expression { get; init; }

    public CellNode(Guid id, Guid workspaceId, string key, decimal value, string? expression)
    {
        Id = id;
        WorkspaceId = workspaceId;
        Key = key;
        Value = value;
        Expression = expression;
    }
}
