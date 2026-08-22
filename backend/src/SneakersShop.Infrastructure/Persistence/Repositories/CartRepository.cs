using Microsoft.EntityFrameworkCore;

using SneakersShop.Application.Abstractions.Repositories;
using SneakersShop.Domain.Carts;

namespace SneakersShop.Infrastructure.Persistence.Repositories;

internal sealed class CartRepository(AppDbContext context) : ICartRepository
{
    public async Task<Cart?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

    public async Task AddAsync(Cart cart, CancellationToken cancellationToken = default)
    {
        await context.Carts.AddAsync(cart, cancellationToken);
    }
}