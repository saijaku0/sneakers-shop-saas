using Microsoft.EntityFrameworkCore;

using SneakersShop.Application.Abstractions.ApplicatonDbContext;
using SneakersShop.Application.Abstractions.Authentication;
using SneakersShop.Application.Abstractions.Queries;
using SneakersShop.Application.Carts.DTOs;
using SneakersShop.Domain.Common.Results;
using SneakersShop.Domain.Consumer.Errors;

namespace SneakersShop.Application.Carts.Queries.GetCart;

public sealed class GetCartQueryHandler(
    IApplicationDbContext context,
    ICurrentUserService userService)
    : IQueryHandler<GetCartQuery, CartDto>
{
    public async Task<Result<CartDto>> Handle(
        GetCartQuery request,
        CancellationToken cancellationToken)
    {
        var userId = userService.GetUserId();
        if (userId is null)
            return Result<CartDto>.Failure(UserErrors.UserIsUnauthorized);

        var items = await (
            from cart in context.Carts.AsNoTracking()
            where cart.UserId == userId.Value
            from ci in cart.Items
            join w in context.WarehouseItems on ci.WarehouseItemId equals w.Id
            join v in context.ProductVariants on w.ProductVariantId equals v.Id
            join p in context.Products on v.ProductId equals p.Id
            join b in context.Brands on p.BrandId equals b.Id
            select new CartItemDto(
                w.Id,
                p.Id,
                p.Model,
                b.Name,
                v.Color,
                w.Size.ValueCm,
                v.PreviewImageUrl,
                p.BasePrice,
                ci.Quantity,
                w.Quantity - w.ReservedQuantity,
                w.Quantity - w.ReservedQuantity >= ci.Quantity))
            .ToListAsync(cancellationToken);

        var total = items.Sum(i => i.UnitPrice * i.Quantity);

        return Result<CartDto>.Success(new CartDto(items, total));
    }
}