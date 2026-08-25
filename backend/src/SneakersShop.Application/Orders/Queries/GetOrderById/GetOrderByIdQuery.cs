using SneakersShop.Application.Abstractions.Queries;
using SneakersShop.Application.Orders.DTOs;

namespace SneakersShop.Application.Orders.Queries.GetOrderById;

public sealed record GetOrderByIdQuery(Guid OrderId) : IQuery<OrderDetailDto>;