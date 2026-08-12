using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SneakersShop.Infrastructure.Persistence.Identity;

namespace SneakersShop.Infrastructure.Persistence.Configurations;

internal sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable($"{nameof(RefreshToken)}s");
        builder.HasKey(rt => rt.TokenId);
        builder.Property(rt => rt.Token)
            .ValueGeneratedNever()
            .HasMaxLength(256)
            .IsRequired();
        builder.HasIndex(rt => rt.Token).IsUnique();
        builder.Property(rt => rt.UserId).IsRequired();
        builder.Property(rt => rt.CreatedAt).IsRequired();
        builder.Property(rt => rt.ExpiresAt).IsRequired();
        builder.Property(rt => rt.IsRevoked).IsRequired();
        builder.HasOne<ApplicationUser>()
               .WithMany()
               .HasForeignKey(rt => rt.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}