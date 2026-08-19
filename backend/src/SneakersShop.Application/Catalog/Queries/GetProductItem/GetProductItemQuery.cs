using SneakersShop.Application.Abstractions.Queries;
using SneakersShop.Application.Catalog.DTOs;

namespace SneakersShop.Application.Catalog.Queries.GetProductItem;

public record GetProductItemQuery(Guid ProductId) : IQuery<ProductDetail>;