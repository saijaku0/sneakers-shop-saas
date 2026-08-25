using MediatR;

using Microsoft.EntityFrameworkCore;

using SneakersShop.Application.Abstractions.ApplicatonDbContext;
using SneakersShop.Application.Abstractions.Authentication;
using SneakersShop.Application.Abstractions.Queries;
using SneakersShop.Application.Orders.DTOs;
using SneakersShop.Application.Profiles.DTOs;
using SneakersShop.Domain.Common.Results;
using SneakersShop.Domain.Consumer.Errors;
using SneakersShop.Domain.Orders.Errors;

namespace SneakersShop.Application.Orders.Queries.GetOrderById;

public sealed class GetOrderByIdQueryHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser)
    : IQueryHandler<GetOrderByIdQuery, OrderDetailDto>
{
    public async Task<Result<OrderDetailDto>> Handle(
        GetOrderByIdQuery request,
        CancellationToken cancellationToken)
    {
        var userId = currentUser.GetUserId();
        if (userId is null)
            return Result<OrderDetailDto>.Failure(UserErrors.UserIsUnauthorized);

        var order = await context.Orders
            .AsNoTracking()
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);
        if (order is null || order.UserId != userId.Value)
            return Result<OrderDetailDto>.Failure(OrderErrors.OrderNotFound);

        var warehouseItemIds = order.OrderItems.Select(i => i.WarehouseItemId).ToList();
        var catalog = await context.WarehouseItems
            .AsNoTracking()
            .Where(w => warehouseItemIds.Contains(w.Id))
            .Select(w => new
            {
                WarehouseItemId = w.Id,
                SizeCm = w.Size.ValueCm,
                Variant = context.ProductVariants.First(v => v.Id == w.ProductVariantId),
            })
            .Select(x => new
            {
                x.WarehouseItemId,
                x.SizeCm,
                x.Variant.Color,
                x.Variant.PreviewImageUrl,
                Product = context.Products.First(p => p.Id == x.Variant.ProductId),
            })
            .Select(x => new
            {
                x.WarehouseItemId,
                x.SizeCm,
                x.Color,
                x.PreviewImageUrl,
                x.Product.Model,
                BrandName = context.Brands.First(b => b.Id == x.Product.BrandId).Name,
            })
            .ToDictionaryAsync(x => x.WarehouseItemId, cancellationToken);

        var items = order.OrderItems
            .Where(oi => catalog.ContainsKey(oi.WarehouseItemId))
            .Select(oi =>
            {
                var c = catalog[oi.WarehouseItemId];
                return new OrderItemDto(
                    oi.WarehouseItemId,
                    c.Model,
                    c.BrandName,
                    c.Color,
                    c.SizeCm,
                    c.PreviewImageUrl,
                    oi.UnitPrice,
                    oi.Quantity,
                    oi.DiscountAmount,
                    oi.TotalPrice);
            })
            .ToList();

        var address = new AddressDto(
            order.ShippingAddress.Country,
            order.ShippingAddress.State,
            order.ShippingAddress.City,
            order.ShippingAddress.Street,
            order.ShippingAddress.HouseNumber,
            order.ShippingAddress.ZipCode);

        var dto = new OrderDetailDto(
            order.Id,
            order.Status.ToString(),
            order.TotalAmount,
            order.PaymentMethod.ToString(),
            order.PaymentDeadline,
            order.CreatedAt,
            address,
            items);

        return Result<OrderDetailDto>.Success(dto);
    }
}