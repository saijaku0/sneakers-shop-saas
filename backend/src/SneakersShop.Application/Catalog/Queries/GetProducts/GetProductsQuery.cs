using SneakersShop.Application.Abstractions.Queries;
using SneakersShop.Application.Catalog.DTOs;
using SneakersShop.Application.Common.PageResults;
using SneakersShop.Domain.Catalog.Enums;

namespace SneakersShop.Application.Catalog.Queries.GetProducts;

public record GetProductsQuery(
    Gender? Gender,
    IReadOnlyList<string>? Categories = null,
    IReadOnlyList<string>? Brands = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    IReadOnlyList<string>? Colors = null,
    IReadOnlyList<decimal>? Sizes = null,
    bool? InStockOnly = null,
    string? SortBy = null,
    int PageNumber = 1,
    int PageSize = 20)
    : IQuery<PageResult<ProductListItem>>;