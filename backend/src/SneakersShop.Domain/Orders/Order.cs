using SneakersShop.Domain.Common.Entities;
using SneakersShop.Domain.Common.Exceptions;
using SneakersShop.Domain.Common.Guards;
using SneakersShop.Domain.Common.Results;
using SneakersShop.Domain.Common.ValueObjects;
using SneakersShop.Domain.Orders.DomainEvents;
using SneakersShop.Domain.Orders.Enums;
using SneakersShop.Domain.Orders.Errors;

namespace SneakersShop.Domain.Orders;

public sealed class Order : AggregateRoot
{
    public Guid UserId { get; private init; }
    public OrderStatus Status { get; private set; }
    public decimal TotalAmount { get; private set; }
    public Address ShippingAddress { get; private init; }
    public PaymentMethod PaymentMethod { get; private init; }
    public DateTimeOffset PaymentDeadline { get; private init; }

    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();
    private readonly List<OrderItem> _orderItems = [];

    private Order() { }
    private Order(
        Guid userId,
        Address shippingAddress,
        PaymentMethod paymentMethod,
        DateTimeOffset paymentDeadline,
        List<OrderItem> items) : base(Guid.CreateVersion7())
    {
        UserId = userId;
        ShippingAddress = shippingAddress;
        PaymentMethod = paymentMethod;
        PaymentDeadline = paymentDeadline;
        Status = OrderStatus.Pending;
        _orderItems = [.. items];

        TotalAmount = _orderItems.Sum(i => i.TotalPrice);
        EnsureInvariant();
    }

    public static Order Create(
        Guid userId,
        Address shippingAddress,
        PaymentMethod paymentMethod,
        List<OrderItem> items,
        DateTimeOffset now)
    {
        Guard.Against.Empty(userId);
        Guard.Against.Null(shippingAddress);
        Guard.Against.NullOrEmpty(items);

        if (!Enum.IsDefined(paymentMethod))
            throw new DomainException($"Invalid payment method: {paymentMethod}");
        // Example: Payment deadline is set to 30 minutes from now. In a real application, this could be configurable.
        var paymentDeadline = now.AddMinutes(30);

        return new Order(userId, shippingAddress, paymentMethod, paymentDeadline, items);
    }

    public Result Pay(DateTimeOffset now)
    {
        if (Status != OrderStatus.Pending)
            return OrderError.InvalidStateTransition(Status, OrderStatus.Paid);

        if (now > PaymentDeadline)
            return OrderError.PaymentDeadlineExpired(PaymentDeadline);

        Status = OrderStatus.Paid;
        Touch();
        AddDomainEvent(new OrderPaid(Id));
        EnsureInvariant();
        return Result.Success();
    }

    public Result Cancel(DateTimeOffset now)
    {
        if (Status == OrderStatus.Cancelled)
            return Result.Success();

        if (Status is OrderStatus.Shipping or OrderStatus.Delivered)
            return OrderError.InvalidStateTransition(Status, OrderStatus.Cancelled);

        Status = OrderStatus.Cancelled;
        Touch();
        AddDomainEvent(new OrderCancelled(Id, now));
        EnsureInvariant();
        return Result.Success();
    }

    public void StartPackaging()
    {
        if (Status == OrderStatus.Packaging)
            return;

        if (Status != OrderStatus.Paid)
            throw new DomainException($"Cannot start packaging. Order is in '{Status}' state.");

        Status = OrderStatus.Packaging;
        Touch();
        EnsureInvariant();
    }

    public void Ship()
    {
        if (Status == OrderStatus.Shipping)
            return;

        if (Status != OrderStatus.Packaging)
            throw new DomainException($"Cannot ship. Order is in '{Status}' state.");

        Status = OrderStatus.Shipping;
        Touch();
        EnsureInvariant();
    }

    public void Deliver()
    {
        if (Status == OrderStatus.Delivered)
            return;

        if (Status != OrderStatus.Shipping)
            throw new DomainException($"Cannot deliver. Order is in '{Status}' state.");

        Status = OrderStatus.Delivered;
        Touch();
        EnsureInvariant();
    }

    private void EnsureInvariant()
    {
        var calculatedAmount = _orderItems.Sum(i => i.TotalPrice);

        if (TotalAmount != calculatedAmount)
            throw new DomainException($"Invariant violation in Order {Id}: TotalAmount ({TotalAmount}) does not match the sum of items ({calculatedAmount}).");
    }
}