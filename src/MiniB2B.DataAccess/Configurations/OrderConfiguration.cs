using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniB2B.Entities.Models;

namespace MiniB2B.DataAccess.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.Property(o => o.OrderNumber)
               .HasDefaultValueSql("NEXT VALUE FOR dbo.OrderNumberSeq")
               .ValueGeneratedOnAdd();

        builder.Property(o => o.TotalAmount)
               .HasPrecision(18, 2);

        builder.HasOne(o => o.User)
               .WithMany(u => u.Orders)
               .HasForeignKey(o => o.UserId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}