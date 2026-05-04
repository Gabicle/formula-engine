namespace FormulaEngine.Api.Models;

public class Formula
{
    public Guid Id { get; init; }
    public Guid CellId { get; init; }
    public Guid WorkspaceId { get; init; }
    public string Expression { get; set; } = null!;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; set; }

    public Cell Cell { get; set; } = null!;
    
    private Formula() { }

    public Formula(Guid cellId, Guid workspaceId, string expression)
    {
        Id = Guid.NewGuid();
        CellId = cellId;
        WorkspaceId = workspaceId;
        Expression = expression;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}