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

        var query = context.Products.AsNoTracking();

        if (request.Gender is not null)
            query = query.Where(p => p.Gender == request.Gender);

        if (!string.IsNullOrWhiteSpace(request.Brand))
            query = query.Where(p =>
                context.Brands.Any(b => b.Id == p.BrandId && b.Name == request.Brand));

        if (!string.IsNullOrWhiteSpace(request.Category))
            query = query.Where(p =>
                context.Categories.Any(c => c.Id == p.CategoryId && c.Name == request.Category));

        if (request.MinPrice is not null)
            query = query.Where(p => p.BasePrice >= request.MinPrice);

        if (request.MaxPrice is not null)
            query = query.Where(p => p.BasePrice <= request.MaxPrice);

        if (!string.IsNullOrWhiteSpace(request.Color))
            query = query.Where(p => context.ProductVariants
                .Any(v => v.ProductId == p.Id && v.Color == request.Color));

        if (request.Size is not null)
            query = query.Where(p => context.ProductVariants
                .Any(v => v.ProductId == p.Id && context.WarehouseItems
                    .Any(w => w.ProductVariantId == v.Id
                           && w.Size.ValueCm == request.Size
                           && w.Quantity - w.ReservedQuantity > 0)));

        if (request.InStockOnly == true)
            query = query.Where(p => context.ProductVariants
                .Any(v => v.ProductId == p.Id && context.WarehouseItems
                    .Any(w => w.ProductVariantId == v.Id
                           && w.Quantity - w.ReservedQuantity > 0)));

        var totalCount = await query.CountAsync(cancellationToken);

        query = request.SortBy switch
        {
            "price_asc" => query.OrderBy(p => p.BasePrice),
            "price_desc" => query.OrderByDescending(p => p.BasePrice),
            "newest" => query.OrderByDescending(p => p.CreatedAt),
            "name" => query.OrderBy(p => p.Model),
            _ => query.OrderBy(p => p.Model)
        };

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductListItem(
                p.Id,
                p.Model,
                context.Brands.First(b => b.Id == p.BrandId).Name,
                p.BasePrice,
                context.ProductVariants
                    .Where(v => v.ProductId == p.Id)
                    .Select(v => v.PreviewImageUrl)
                    .FirstOrDefault() ?? string.Empty,
                context.ProductVariants.Count(v => v.ProductId == p.Id)))
            .ToListAsync(cancellationToken);

        return Result<PageResult<ProductListItem>>.Success(
            new PageResult<ProductListItem>(
                Items: items,
                PageNumber: pageNumber,
                PageSize: pageSize,
                TotalCount: totalCount));
    }
}