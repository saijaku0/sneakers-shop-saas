using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SneakersShop.Domain.Catalog;
using SneakersShop.Domain.Warehouse;

namespace SneakersShop.Infrastructure.Persistence.Configurations;

internal sealed class WarehouseItemConfiguration : IEntityTypeConfiguration<WarehouseItem>
{
    public void Configure(EntityTypeBuilder<WarehouseItem> builder)
    {
        builder.ToTable($"{nameof(WarehouseItem)}s");
        builder.HasKey(w => w.Id);
        builder.ComplexProperty(w => w.Size, size
            => size.Property(s => s.ValueCm)
                .HasColumnName("Size")
                .HasPrecision(4, 1)
                .IsRequired());
        builder.Property(w => w.Quantity).IsRequired();
        builder.Property(w => w.ReservedQuantity).IsRequired();
        builder.Property(w => w.RowVersion).IsRowVersion();
        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(w => w.ProductId);
        builder.HasIndex(w => new { w.ProductId })
            .HasDatabaseName("IX_WarehouseItems_ProductId");
    }
}