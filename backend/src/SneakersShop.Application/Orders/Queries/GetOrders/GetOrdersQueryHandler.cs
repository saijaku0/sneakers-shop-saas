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
    private const int PreviewCount = 3;

    public async Task<Result<IReadOnlyList<OrderSummaryDto>>> Handle(
        GetOrdersQuery request,
        CancellationToken cancellationToken)
    {
        var userId = currentUser.GetUserId();
        if (userId is null)
            return Result<IReadOnlyList<OrderSummaryDto>>.Failure(UserErrors.UserIsUnauthorized);

        var orders = await context.Orders
            .AsNoTracking()
            .Where(o => o.UserId == userId.Value)
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new
            {
                o.Id,
                o.Status,
                o.TotalAmount,
                o.CreatedAt,
                Items = o.OrderItems
                    .OrderBy(i => i.Id)
                    .Select(i => i.WarehouseItemId)
                    .ToList()
            })
            .ToListAsync(cancellationToken);

        if (orders.Count == 0)
            return Result<IReadOnlyList<OrderSummaryDto>>.Success([]);

        var allWarehouseItemIds = orders
            .SelectMany(o => o.Items)
            .Distinct()
            .ToList();

        var catalog = await (
            from w in context.WarehouseItems.AsNoTracking()
            where allWarehouseItemIds.Contains(w.Id)
            join v in context.ProductVariants on w.ProductVariantId equals v.Id
            join p in context.Products on v.ProductId equals p.Id
            join b in context.Brands on p.BrandId equals b.Id
            select new
            {
                WarehouseItemId = w.Id,
                v.PreviewImageUrl,
                Title = b.Name + " " + p.Model
            })
            .ToDictionaryAsync(x => x.WarehouseItemId, cancellationToken);

        var dtos = orders.Select(o =>
        {
            var enriched = o.Items
                .Where(id => catalog.ContainsKey(id))
                .Select(id => catalog[id])
                .ToList();

            var previewImages = enriched
                .Take(PreviewCount)
                .Select(x => x.PreviewImageUrl)
                .ToList();

            var names = enriched.Select(x => x.Title).ToList();
            var shown = names.Take(2).ToList();
            var remaining = names.Count - shown.Count;
            var previewText = string.Join(", ", shown) + (remaining > 0 ? $" +{remaining}" : "");

            return new OrderSummaryDto(
                o.Id,
                o.Status.ToString(),
                o.TotalAmount,
                o.CreatedAt,
                o.Items.Count,
                previewImages,
                previewText);
        }).ToList();

        return Result<IReadOnlyList<OrderSummaryDto>>.Success(dtos);
    }
}