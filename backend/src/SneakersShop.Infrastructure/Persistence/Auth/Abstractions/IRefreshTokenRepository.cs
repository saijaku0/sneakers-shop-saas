using SneakersShop.Infrastructure.Persistence.Identity;

namespace SneakersShop.Infrastructure.Persistence.Auth.Abstractions;

internal interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task UpdateAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
    Task RemoveExpiredOrRevokedAsync(CancellationToken cancellationToken = default);
    Task RevokeAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}