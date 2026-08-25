using SneakersShop.Application.Abstractions.Queries;
using SneakersShop.Application.Orders.DTOs;

namespace SneakersShop.Application.Orders.Queries.GetOrders;

public sealed record GetOrdersQuery() : IQuery<IReadOnlyList<OrderSummaryDto>>;