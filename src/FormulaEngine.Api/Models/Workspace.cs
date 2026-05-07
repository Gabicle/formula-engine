
namespace FormulaEngine.Api.Models;

public class Workspace
{

    public Guid Id { get; init; }
    public string TenantId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public DateTime CreatedAt { get; init; }

    public ICollection<Cell> Cells { get; set; } = new List<Cell>();
    public ICollection<Dependency> Dependencies { get; set; } = new List<Dependency>();

    private Workspace() { }

    public Workspace(string tenantId, string name)
    {
        Id = Guid.NewGuid();
        TenantId = tenantId;
        Name = name;
        CreatedAt = DateTime.UtcNow;
    }


}
