using FormulaEngine.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FormulaEngine.Api.Data.Configurations;

public class FormulaConfiguration : IEntityTypeConfiguration<Formula>
{
    public void Configure(EntityTypeBuilder<Formula> entity)
    {
        entity.HasKey(f => f.Id);

        entity.Property(f => f.Expression)
            .IsRequired();

        entity.HasOne(f => f.Cell)
            .WithOne(c => c.Formula)
            .HasForeignKey<Formula>(f => f.CellId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}