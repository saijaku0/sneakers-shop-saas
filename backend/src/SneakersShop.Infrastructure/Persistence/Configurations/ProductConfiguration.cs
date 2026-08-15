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
        builder.Property(p => p.IsActive).IsRequired();
        builder.Property(p => p.Gender)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(p => p.BasePrice)
            .HasPrecision(18, 2);

        builder.HasOne<Brand>()
            .WithMany()
            .HasForeignKey(p => p.BrandId);

        builder.HasIndex(p => p.BrandId);
    }
}