using Microsoft.EntityFrameworkCore;

using SneakersShop.Application.Abstractions.ApplicatonDbContext;
using SneakersShop.Application.Abstractions.Queries;
using SneakersShop.Application.Catalog.DTOs;
using SneakersShop.Domain.Catalog.Errors;
using SneakersShop.Domain.Common.Results;

namespace SneakersShop.Application.Catalog.Queries.GetProductItem;

public sealed class GetProductItemQueryHandler(IApplicationDbContext context)
    : IQueryHandler<GetProductItemQuery, ProductDetail>
{
    public async Task<Result<ProductDetail>> Handle(
        GetProductItemQuery request,
        CancellationToken cancellationToken)
    {
        var detail = await context.Products
            .AsNoTracking()
            .Where(p => p.Id == request.ProductId)
            .Select(p => new ProductDetail(
                p.Id,
                p.Model,
                context.Brands.First(b => b.Id == p.BrandId).Name,
                context.Categories.First(c => c.Id == p.CategoryId).Name,
                p.Gender,
                p.BasePrice,
                p.Description,
                context.ProductVariants
                    .Where(v => v.ProductId == p.Id)
                    .OrderBy(v => v.Id)
                    .Select(v => new VariantDetail(
                        v.Id,
                        v.Color,
                        v.Images.Select(i => i.Url).ToList(),
                        context.WarehouseItems
                            .Where(w => w.ProductVariantId == v.Id)
                            .Select(w => new SizeAvailability(
                                w.Id,
                                w.Size.ValueCm,
                                w.Quantity - w.ReservedQuantity > 0))
                            .ToList()))
                    .ToList()))
            .FirstOrDefaultAsync(cancellationToken);

        if (detail is null)
            return Result<ProductDetail>.Failure(ProductErrors.ProductNotFound(request.ProductId));

        return Result<ProductDetail>.Success(detail);
    }
}