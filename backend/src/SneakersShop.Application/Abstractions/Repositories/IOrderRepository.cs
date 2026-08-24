using SneakersShop.Domain.Orders;

namespace SneakersShop.Application.Abstractions.Repositories;

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken cancellation = default);
}