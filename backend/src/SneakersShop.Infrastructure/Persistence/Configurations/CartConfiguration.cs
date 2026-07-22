using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SneakersShop.Domain.Carts;

namespace SneakersShop.Infrastructure.Persistence.Configurations;

internal sealed class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable($"{nameof(Cart)}s");
        builder.HasKey(c => c.Id);
        builder.HasMany(c => c.Items)
            .WithOne()
            .HasForeignKey("CartId")
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(c => c.Items)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Property(c => c.UserId).IsRequired();
        builder.HasIndex(c => c.UserId).IsUnique();
    }
}