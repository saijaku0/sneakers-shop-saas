using Microsoft.EntityFrameworkCore;

using SneakersShop.Application.Abstractions.ApplicatonDbContext;
using SneakersShop.Application.Abstractions.Authentication;
using SneakersShop.Application.Abstractions.Queries;
using SneakersShop.Application.Orders.DTOs;
using SneakersShop.Domain.Common.Results;
using SneakersShop.Domain.Consumer.Errors;

namespace SneakersShop.Application.Orders.Queries.GetOrders;

public sealed class GetOrdersQueryHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser)
    : IQueryHandler<GetOrdersQuery, IReadOnlyList<OrderSummaryDto>>
{
    public async Task<Result<IReadOnlyList<OrderSummaryDto>>> Handle(
        GetOrdersQuery request,
        CancellationToken cancellationToken)
    {
        var userId = currentUser.GetUserId();
        if (userId is null)
            return Result<IReadOnlyList<OrderSummaryDto>>.Failure(UserErrors.UserIsUnauthorized);

        var orders = await context.Orders
            .AsNoTracking()
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new
            {
                o.Id,
                o.Status,
                o.TotalAmount,
                o.CreatedAt,
                o.OrderItems.Count
            })
            .ToListAsync(cancellationToken);

        var dtos = orders.Select(o => new OrderSummaryDto(
                o.Id,
                o.Status.ToString(),
                o.TotalAmount,
                o.CreatedAt,
                o.Count))
            .ToList();

        return Result<IReadOnlyList<OrderSummaryDto>>.Success(dtos);
    }
}