namespace FormulaEngine.Api.Models;

public class Tenant
{
    public string Id { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string CultureCode { get; init; } = null!;
    public bool IsActive { get; init; }

    public Tenant(string id, string name, string cultureCode)
    {
        Id          = id;
        Name        = name;
        CultureCode = cultureCode;
        IsActive    = true;
    }

    private Tenant() { }
}