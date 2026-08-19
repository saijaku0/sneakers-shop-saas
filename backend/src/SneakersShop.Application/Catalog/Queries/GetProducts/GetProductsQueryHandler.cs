using Microsoft.EntityFrameworkCore;

using SneakersShop.Application.Abstractions.ApplicatonDbContext;
using SneakersShop.Application.Abstractions.Queries;
using SneakersShop.Application.Catalog.DTOs;
using SneakersShop.Application.Common.PageResults;
using SneakersShop.Domain.Common.Results;

namespace SneakersShop.Application.Catalog.Queries.GetProducts;

public sealed class GetProductsQueryHandler(IApplicationDbContext context)
    : IQueryHandler<GetProductsQuery, PageResult<ProductListItem>>
{
    public async Task<Result<PageResult<ProductListItem>>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var pageNumber = Math.Max(request.PageNumber, 1);

        var query = context.Products
            .AsNoTracking()
            .ApplyFilters(request, context);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .ApplySorting(request.SortBy)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ProjectToListItem(context)
            .ToListAsync(cancellationToken);

        return Result<PageResult<ProductListItem>>.Success(
            new PageResult<ProductListItem>(
                Items: items,
                PageNumber: pageNumber,
                PageSize: pageSize,
                TotalCount: totalCount));
    }
}