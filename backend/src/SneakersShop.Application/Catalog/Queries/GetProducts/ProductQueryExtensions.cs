using SneakersShop.Application.Abstractions.ApplicatonDbContext;
using SneakersShop.Application.Catalog.DTOs;
using SneakersShop.Domain.Catalog;

namespace SneakersShop.Application.Catalog.Queries.GetProducts;

internal static class ProductQueryExtensions
{
    public static IQueryable<Product> ApplyFilters(
        this IQueryable<Product> query,
        GetProductsQuery request,
        IApplicationDbContext context)
    {
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

        return query;
    }

    public static IQueryable<Product> ApplySorting(
        this IQueryable<Product> query,
        string? sortBy)
        => sortBy switch
        {
            "price_asc" => query.OrderBy(p => p.BasePrice),
            "price_desc" => query.OrderByDescending(p => p.BasePrice),
            "newest" => query.OrderByDescending(p => p.CreatedAt),
            "name" => query.OrderBy(p => p.Model),
            _ => query.OrderBy(p => p.Model)
        };

    public static IQueryable<ProductListItem> ProjectToListItem(
        this IQueryable<Product> query,
        IApplicationDbContext context)
        => query.Select(p => new ProductListItem(
            p.Id,
            p.Model,
            context.Brands.First(b => b.Id == p.BrandId).Name,
            p.BasePrice,
            context.ProductVariants
                .Where(v => v.ProductId == p.Id)
                .OrderBy(v => v.Id)
                .Take(4)
                .Select(v => new ProductVariantPreviewDto(
                    v.Id,
                    v.Color,
                    v.PreviewImageUrl))
                .ToList(),
            context.ProductVariants.Count(v => v.ProductId == p.Id)));
}