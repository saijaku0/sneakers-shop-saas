using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SneakersShop.Domain.Brands;
using SneakersShop.Domain.Catalog;

namespace SneakersShop.Infrastructure.Persistence.Configurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable($"{nameof(Product)}s");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Model).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Description).IsRequired().HasMaxLength(2000);
        builder.Property(p => p.Price).HasPrecision(18, 2);
        builder.Property(p => p.IsActive).IsRequired();
        builder.OwnsMany(p => p.Images, img =>
        {
            img.ToJson();
            img.Property(i => i.Url);
        });
        builder.Navigation(p => p.Images).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasOne<Brand>()
            .WithMany()
            .HasForeignKey(p => p.BrandId);

        builder.HasIndex(p => p.BrandId);
    }
}