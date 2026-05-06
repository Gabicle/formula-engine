using FormulaEngine.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FormulaEngine.Api.Data;

public class FormulaEngineContext : DbContext
{
    public FormulaEngineContext(DbContextOptions<FormulaEngineContext> options)
        : base(options) { }

    public DbSet<Workspace> Workspaces => Set<Workspace>();
    public DbSet<Cell> Cells => Set<Cell>();
    public DbSet<Formula> Formulas => Set<Formula>();
    public DbSet<Dependency> Dependencies => Set<Dependency>();
    public DbSet<Tenant> Tenants => Set<Tenant>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FormulaEngineContext).Assembly);
    }
}