namespace FormulaEngine.Api.Models;

public class Dependency
{
    public Guid Id { get; init; }
    public Guid WorkspaceId { get; init; }
    public Guid FromCellId { get; init; }
    public Guid ToCellId { get; init; }
    public DateTime CreatedAt { get; init; }

    public Workspace Workspace { get; set; } = null!;
    public Cell FromCell { get; set; } = null!;
    public Cell ToCellNav { get; set; } = null!;
    
    private Dependency() { }

    public Dependency(Guid workspaceId, Guid fromCellId, Guid toCellId)
    {
        Id = Guid.NewGuid();
        WorkspaceId = workspaceId;
        FromCellId = fromCellId;
        ToCellId = toCellId;
        CreatedAt = DateTime.UtcNow;
    }
}