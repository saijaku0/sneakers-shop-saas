using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SneakersShop.Domain.Carts;
using SneakersShop.Domain.Warehouse;

namespace SneakersShop.Infrastructure.Persistence.Configurations;

internal sealed class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable($"{nameof(CartItem)}s");
        builder.HasKey(ci => ci.Id);
        builder.Property(ci => ci.WarehouseItemId);
        builder.HasOne<WarehouseItem>()
            .WithMany()
            .HasForeignKey(ci => ci.WarehouseItemId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Property(ci => ci.Quantity).IsRequired();
    }
}