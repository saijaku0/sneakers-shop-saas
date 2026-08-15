using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SneakersShop.Domain.Catalog;

namespace SneakersShop.Infrastructure.Persistence.Configurations;

internal sealed class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.ToTable($"{nameof(ProductVariant)}s");
        builder.HasKey(v => v.Id);
        builder.Property(v => v.Color).HasMaxLength(100).IsRequired();
        builder.OwnsMany(v => v.Images, img => img.ToJson());
        builder.Navigation(v => v.Images).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(v => v.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(v => new { v.ProductId, v.Color }).IsUnique();
    }
}