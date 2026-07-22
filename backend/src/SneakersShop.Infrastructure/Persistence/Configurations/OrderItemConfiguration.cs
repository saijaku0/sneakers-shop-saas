using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SneakersShop.Domain.Orders;
using SneakersShop.Domain.Warehouse;

namespace SneakersShop.Infrastructure.Persistence.Configurations;

internal sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable($"{nameof(OrderItem)}s");
        builder.HasKey(oi => oi.Id);

        builder.Property(oi => oi.Quantity).IsRequired();
        builder.Property(oi => oi.UnitPrice).HasPrecision(18, 2);
        builder.Property(oi => oi.DiscountAmount).HasPrecision(18, 2);

        builder.HasOne<WarehouseItem>()
            .WithMany()
            .HasForeignKey(oi => oi.WarehouseItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(oi => oi.TotalPrice);
    }
}