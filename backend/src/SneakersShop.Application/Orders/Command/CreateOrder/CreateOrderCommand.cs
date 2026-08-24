using SneakersShop.Application.Abstractions.Commands;
using SneakersShop.Domain.Common.ValueObjects;
using SneakersShop.Domain.Orders.Enums;

namespace SneakersShop.Application.Orders.Command.CreateOrder;

public sealed record CreateOrderCommand(
    Address? ShippingAddress,
    PaymentMethod PaymentMethod) : ICommand<Guid>;