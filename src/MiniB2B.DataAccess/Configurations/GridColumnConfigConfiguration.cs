using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniB2B.Entities.Models;

namespace MiniB2B.DataAccess.Configurations;

public class GridColumnConfigConfiguration : IEntityTypeConfiguration<GridColumnConfig>
{
    public void Configure(EntityTypeBuilder<GridColumnConfig> builder)
    {
        builder.Property(g => g.RenderType)
               .HasConversion<string>()
               .HasMaxLength(30)
               .IsUnicode(false);

        builder.Property(g => g.GridKey).HasMaxLength(50).IsUnicode(false);
        builder.Property(g => g.FieldName).HasMaxLength(50).IsUnicode(false);
        builder.Property(g => g.Width).HasMaxLength(10).IsUnicode(false);
        builder.Property(g => g.Alignment).HasMaxLength(10).IsUnicode(false);
    }
}