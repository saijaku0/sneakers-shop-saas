using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SneakersShop.Domain.Consumer;

namespace SneakersShop.Infrastructure.Persistence.Configurations;

internal sealed class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable($"{nameof(UserProfile)}s");
        builder.HasKey(up => up.Id);
        builder.OwnsOne(up => up.DefaultAddress, address =>
        {
            address.Property(a => a.Street)
                .HasMaxLength(200)
                .IsRequired(false);
            address.Property(a => a.City)
                .HasMaxLength(100)
                .IsRequired(false);
            address.Property(a => a.State)
                .HasMaxLength(100)
                .IsRequired(false);
            address.Property(a => a.HouseNumber)
                .HasMaxLength(100)
                .IsRequired(false);
            address.Property(a => a.ZipCode)
                .HasMaxLength(20)
                .IsRequired(false);
            address.Property(a => a.Country)
                .HasMaxLength(100)
                .IsRequired(false);
        });
        builder.Property(up => up.IsFlagged)
            .IsRequired();
    }
}