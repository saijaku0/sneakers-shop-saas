using SneakersShop.Application.Abstractions.Repositories;
using SneakersShop.Domain.Warehouse;

namespace SneakersShop.Infrastructure.Persistence.Repositories;

internal sealed class WarehouseItemRepository(AppDbContext appDbContext) : IWarehouseItemRepository
{
    public async Task<WarehouseItem?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        await appDbContext.WarehouseItems.FindAsync([id], cancellationToken);
}