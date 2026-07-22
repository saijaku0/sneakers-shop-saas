using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SneakersShop.Domain.Orders;

namespace SneakersShop.Infrastructure.Persistence.Configurations;

internal sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable($"{nameof(Order)}s");
        builder.HasKey(o => o.Id);
        builder.Property(o => o.UserId).IsRequired();
        builder.Property(o => o.Status)
            .HasConversion<string>()
            .HasMaxLength(20);
        builder.Property(o => o.TotalAmount)
            .HasPrecision(18, 2);
        builder.ComplexProperty(o => o.ShippingAddress, sa =>
        {
            sa.Property(a => a.Country).HasMaxLength(50);
            sa.Property(a => a.State).HasMaxLength(50);
            sa.Property(a => a.City).HasMaxLength(50);
            sa.Property(a => a.Street).HasMaxLength(100);
            sa.Property(a => a.HouseNumber).HasMaxLength(20);
            sa.Property(a => a.ZipCode).HasMaxLength(20);
        });
        builder.Property(o => o.PaymentMethod)
            .HasConversion<string>()
            .HasMaxLength(20);
        builder.Property(o => o.PaymentDeadline)
            .HasColumnType("datetimeoffset(0)");
        builder.HasMany(o => o.OrderItems)
            .WithOne()
            .HasForeignKey("OrderId")
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(o => o.OrderItems)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}