using SneakersShop.Application.Abstractions.Queries;
using SneakersShop.Application.Brands.DTOs;

namespace SneakersShop.Application.Brands.Queries.GetBrands;

public record GetBrandsQuery() : IQuery<IReadOnlyList<BrandDto>>;