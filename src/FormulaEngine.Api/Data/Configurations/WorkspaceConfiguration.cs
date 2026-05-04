using FormulaEngine.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FormulaEngine.Api.Data.Configurations;

public class WorkspaceConfiguration : IEntityTypeConfiguration<Workspace>
{
    public void Configure(EntityTypeBuilder<Workspace> entity)
    {
        entity.HasKey(w => w.Id);

        entity.Property(w => w.TenantId)
            .IsRequired()
            .HasMaxLength(256);

        entity.Property(w => w.Name)
            .IsRequired()
            .HasMaxLength(256);

        entity.HasIndex(w => w.TenantId);
    }
}