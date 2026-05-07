using FormulaEngine.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FormulaEngine.Api.Data.Configurations;

public class CellConfiguration : IEntityTypeConfiguration<Cell>
{
    public void Configure(EntityTypeBuilder<Cell> entity)
    {
        entity.HasKey(c => c.Id);

        entity.Property(c => c.Key)
            .IsRequired()
            .HasMaxLength(256);

        entity.Property(c => c.Value)
            .HasPrecision(28, 10);

        entity.HasIndex(c => new { c.WorkspaceId, c.Key })
            .IsUnique();

        entity.HasOne(c => c.Workspace)
            .WithMany(w => w.Cells)
            .HasForeignKey(c => c.WorkspaceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
