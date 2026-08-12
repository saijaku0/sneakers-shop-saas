using Microsoft.EntityFrameworkCore;

using SneakersShop.Application.Abstractions.Repositories;
using SneakersShop.Infrastructure.Persistence.Auth.Abstractions;
using SneakersShop.Infrastructure.Persistence.Identity;

namespace SneakersShop.Infrastructure.Persistence.Repositories;

internal sealed class RefreshTokenRepository(
    AppDbContext context,
    IUnitOfWork unitOfWork) : IRefreshTokenRepository
{
    public async Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        await context.RefreshTokens.AddAsync(refreshToken, cancellationToken);
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default) =>
        await context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);

    public async Task UpdateAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        context.RefreshTokens.Update(refreshToken);
    }

    public async Task RemoveExpiredOrRevokedAsync(CancellationToken cancellationToken = default)
    {
        var cutoffDate = DateTimeOffset.UtcNow.AddDays(-7);

        var oldTokens = await context.RefreshTokens
        .Where(rt => rt.ExpiresAt < cutoffDate)
        .ToListAsync(cancellationToken);

        if (oldTokens.Count != 0)
            context.RefreshTokens.RemoveRange(oldTokens);
    }

    public async Task RevokeAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var activeTokens = await context.RefreshTokens
            .Where(rt => rt.UserId == userId && !rt.IsRevoked)
            .ToListAsync(cancellationToken);

        foreach (var token in activeTokens)
        {
            token.Revoke();
        }
    }
}