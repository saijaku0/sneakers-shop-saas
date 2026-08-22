using SneakersShop.Domain.Carts;

namespace SneakersShop.Application.Abstractions.Repositories;

public interface ICartRepository
{
    Task<Cart?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(Cart cart, CancellationToken cancellationToken = default);
}