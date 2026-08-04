using SneakersShop.Domain.Warehouse;

namespace SneakersShop.Application.Abstractions.Repositories;

public interface IWarehouseItemRepository
{
    Task<WarehouseItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}