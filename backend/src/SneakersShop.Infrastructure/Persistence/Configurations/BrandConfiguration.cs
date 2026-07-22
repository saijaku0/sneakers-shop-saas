using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SneakersShop.Domain.Brands;

namespace SneakersShop.Infrastructure.Persistence.Configurations;

internal sealed class BrandConfiguration : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.ToTable($"{nameof(Brand)}s");
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Name).IsRequired();
    }
}