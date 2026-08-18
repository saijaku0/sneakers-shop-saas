using SneakersShop.Application.Abstractions.Queries;
using SneakersShop.Application.Catalog.DTOs;
using SneakersShop.Application.Common.PageResults;
using SneakersShop.Domain.Catalog.Enums;

namespace SneakersShop.Application.Catalog.Queries.GetProducts;

public record GetProductsQuery(
    Gender? Gender,
    string? Category,
    string? Brand,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    string? Color = null,
    decimal? Size = null,
    bool? InStockOnly = null,
    string? SortBy = null, // "price_asc" | "price_desc" | "newest" | "name"
    int PageNumber = 1,
    int PageSize = 20)
    : IQuery<PageResult<ProductListItem>>;