using Microsoft.EntityFrameworkCore;

using SneakersShop.Application.Abstractions.ApplicatonDbContext;
using SneakersShop.Application.Abstractions.Queries;
using SneakersShop.Application.Catalog.DTOs;
using SneakersShop.Domain.Common.Results;

namespace SneakersShop.Application.Catalog.Queries.GetFilters;

internal class GetFiltersQueryHandler(IApplicationDbContext context)
    : IQueryHandler<GetFiltersQuery, FiltersDto>
{
    public async Task<Result<FiltersDto>> Handle(
        GetFiltersQuery request,
        CancellationToken cancellationToken)
    {
        var brands = await context.Brands
            .Select(b => b.Name)
            .OrderBy(n => n)
            .ToListAsync(cancellationToken);

        var categories = await context.Categories
            .Select(c => c.Name)
            .OrderBy(n => n)
            .ToListAsync(cancellationToken);

        var colors = await context.ProductVariants
            .Select(v => v.Color)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync(cancellationToken);

        var sizes = await context.WarehouseItems
            .Select(w => w.Size.ValueCm)
            .Distinct()
            .OrderBy(s => s)
            .ToListAsync(cancellationToken);

        var hasProducts = await context.Products.AnyAsync(cancellationToken);

        var minPrice = hasProducts
            ? await context.Products.MinAsync(p => p.BasePrice, cancellationToken)
            : 0m;

        var maxPrice = hasProducts
            ? await context.Products.MaxAsync(p => p.BasePrice, cancellationToken)
            : 0m;

        return Result<FiltersDto>.Success(new FiltersDto(
            brands,
            categories,
            colors,
            sizes,
            new PriceRangeDto(minPrice, maxPrice)));
    }
}