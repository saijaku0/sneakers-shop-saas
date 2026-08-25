using Microsoft.EntityFrameworkCore;

using SneakersShop.Application.Abstractions.ApplicatonDbContext;
using SneakersShop.Application.Abstractions.Queries;
using SneakersShop.Application.Brands.DTOs;
using SneakersShop.Domain.Brands;
using SneakersShop.Domain.Common.Results;

namespace SneakersShop.Application.Brands.Queries.GetBrands;

public sealed class GetBrandsQueryHandler(IApplicationDbContext context)
    : IQueryHandler<GetBrandsQuery, IReadOnlyList<BrandDto>>
{
    public async Task<Result<IReadOnlyList<BrandDto>>> Handle(
        GetBrandsQuery query,
        CancellationToken cancellationToken = default)
    {
        var brands = await context.Brands
            .AsNoTracking()
            .OrderBy(b => b.Name)
            .Select(b => new BrandDto(b.Id, b.Name))
            .ToListAsync(cancellationToken);
        return Result<IReadOnlyList<BrandDto>>.Success(brands);
    }
}