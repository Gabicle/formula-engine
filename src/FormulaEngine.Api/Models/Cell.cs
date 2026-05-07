namespace FormulaEngine.Api.Models;

public class Cell
{
    public Guid Id { get; init; }
    public Guid WorkspaceId { get; init; }
    public string Key { get; set; } = null!;
    public decimal Value { get; set; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; set; }

    public Workspace Workspace { get; set; } = null!;
    public Formula? Formula { get; set; }
    public ICollection<Dependency> IncomingDependencies { get; set; } = new List<Dependency>();
    public ICollection<Dependency> OutgoingDependencies { get; set; } = new List<Dependency>();


    private Cell() { }

    public Cell(Guid workspaceId, string key, decimal value)
    {
        Id = Guid.NewGuid();
        WorkspaceId = workspaceId;
        Key = key;
        Value = value;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

}
