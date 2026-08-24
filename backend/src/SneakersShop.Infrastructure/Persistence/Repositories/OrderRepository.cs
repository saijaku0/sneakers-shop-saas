using SneakersShop.Application.Abstractions.Repositories;
using SneakersShop.Domain.Orders;

namespace SneakersShop.Infrastructure.Persistence.Repositories;

public sealed class OrderRepository(AppDbContext context) : IOrderRepository
{
    public Task AddAsync(Order order, CancellationToken cancellation = default)
    {
        context.Orders.Add(order);
        return Task.CompletedTask;
    }
}