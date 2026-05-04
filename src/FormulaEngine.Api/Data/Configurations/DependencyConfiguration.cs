using FormulaEngine.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FormulaEngine.Api.Data.Configurations;

public class DependencyConfiguration : IEntityTypeConfiguration<Dependency>
{
    public void Configure(EntityTypeBuilder<Dependency> entity)
    {
        entity.HasKey(d => d.Id);

        entity.HasIndex(d => new { d.WorkspaceId, d.FromCellId, d.ToCellId })
            .IsUnique();

        entity.HasOne(d => d.Workspace)
            .WithMany(w => w.Dependencies)
            .HasForeignKey(d => d.WorkspaceId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(d => d.FromCell)
            .WithMany(c => c.OutgoingDependencies)
            .HasForeignKey(d => d.FromCellId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(d => d.ToCellNav)
            .WithMany(c => c.IncomingDependencies)
            .HasForeignKey(d => d.ToCellId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}