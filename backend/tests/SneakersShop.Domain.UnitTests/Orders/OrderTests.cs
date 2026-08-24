using FluentAssertions;

using SneakersShop.Domain.Common.Exceptions;
using SneakersShop.Domain.Common.ValueObjects;
using SneakersShop.Domain.Orders;
using SneakersShop.Domain.Orders.DomainEvents;
using SneakersShop.Domain.Orders.Enums;

namespace SneakersShop.Domain.UnitTests.Orders;

public class OrderTests
{
    private static readonly decimal DiscountRate = 0.1m;

    private static Address CreateAddress()
        => new("Germany", null, "Hof", "Main St", "1", "95028");

    private static Order NewOrder(DateTimeOffset now)
        => Order.Create(Guid.CreateVersion7(), CreateAddress(), PaymentMethod.CreditCard, now);

    private static Order OrderWithItem(DateTimeOffset now, int quantity = 1, decimal unitPrice = 100m)
    {
        var order = NewOrder(now);
        order.AddItem(Guid.CreateVersion7(), quantity, unitPrice, unitPrice * quantity * DiscountRate);
        return order;
    }

    private static Order OrderInStatus(OrderStatus status, DateTimeOffset now)
    {
        var order = OrderWithItem(now);

        switch (status)
        {
            case OrderStatus.Pending:
                return order;
            case OrderStatus.Cancelled:
                order.Cancel(now);
                return order;
            case OrderStatus.Paid:
                order.Pay(now);
                return order;
            case OrderStatus.Packaging:
                order.Pay(now); order.StartPackaging();
                return order;
            case OrderStatus.Shipping:
                order.Pay(now); order.StartPackaging(); order.Ship();
                return order;
            case OrderStatus.Delivered:
                order.Pay(now); order.StartPackaging(); order.Ship(); order.Deliver();
                return order;
            default:
                throw new ArgumentOutOfRangeException(nameof(status));
        }
    }

    [Fact]
    public void Create_WithValidData_ReturnsPendingEmptyOrder()
    {
        var now = DateTimeOffset.UtcNow;

        var order = NewOrder(now);

        order.Status.Should().Be(OrderStatus.Pending);
        order.PaymentDeadline.Should().Be(now.AddMinutes(30));
        order.PaymentMethod.Should().Be(PaymentMethod.CreditCard);
        order.ShippingAddress.Should().NotBeNull();
        order.OrderItems.Should().BeEmpty();
        order.TotalAmount.Should().Be(0m);
        order.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Create_WithEmptyUserId_Throws()
    {
        var now = DateTimeOffset.UtcNow;
        var act = () => Order.Create(Guid.Empty, CreateAddress(), PaymentMethod.CreditCard, now);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithNullAddress_Throws()
    {
        var now = DateTimeOffset.UtcNow;
        var act = () => Order.Create(Guid.CreateVersion7(), null!, PaymentMethod.CreditCard, now);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithUndefinedPaymentMethod_Throws()
    {
        var now = DateTimeOffset.UtcNow;
        var act = () => Order.Create(Guid.CreateVersion7(), CreateAddress(), (PaymentMethod)999, now);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void AddItem_UpdatesTotalAmount()
    {
        var now = DateTimeOffset.UtcNow;
        var order = NewOrder(now);

        var result = order.AddItem(Guid.CreateVersion7(), 2, 100m, 20m);

        result.IsSuccess.Should().BeTrue();
        order.OrderItems.Should().ContainSingle();
        order.TotalAmount.Should().Be(180m);
    }

    [Fact]
    public void AddItem_MultipleItems_SumsTotalAmount()
    {
        var now = DateTimeOffset.UtcNow;
        var order = NewOrder(now);

        order.AddItem(Guid.CreateVersion7(), 1, 100m, 10m);
        order.AddItem(Guid.CreateVersion7(), 2, 50m, 10m);

        order.OrderItems.Should().HaveCount(2);
        order.TotalAmount.Should().Be(180m);
    }

    [Fact]
    public void AddItem_DuplicateWarehouseItem_Fails()
    {
        var now = DateTimeOffset.UtcNow;
        var order = NewOrder(now);
        var warehouseItemId = Guid.CreateVersion7();

        order.AddItem(warehouseItemId, 1, 100m, 0m);
        var result = order.AddItem(warehouseItemId, 1, 100m, 0m);

        result.IsFailure.Should().BeTrue();
        order.OrderItems.Should().ContainSingle();
    }

    [Fact]
    public void AddItem_WhenNotPending_Fails()
    {
        var now = DateTimeOffset.UtcNow;
        var order = OrderWithItem(now);
        order.Pay(now);

        var result = order.AddItem(Guid.CreateVersion7(), 1, 100m, 0m);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Pay_FromPending_BeforeDeadline_SucceedsAndRaisesEvent()
    {
        var now = DateTimeOffset.UtcNow;
        var order = OrderWithItem(now);

        var result = order.Pay(now.AddMinutes(5));

        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Paid);
        order.DomainEvents.Should().ContainSingle(e => e is OrderPaid);
    }

    [Fact]
    public void Pay_AfterDeadline_Fails()
    {
        var now = DateTimeOffset.UtcNow;
        var order = OrderWithItem(now);

        var result = order.Pay(now.AddMinutes(31));

        result.IsFailure.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Pending);
    }

    [Fact]
    public void Pay_Twice_IsIdempotent()
    {
        var now = DateTimeOffset.UtcNow;
        var order = OrderWithItem(now);
        order.Pay(now);
        order.ClearDomainEvents();

        var result = order.Pay(now);

        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Paid);
    }

    [Fact]
    public void Pay_FromCancelled_Fails()
    {
        var now = DateTimeOffset.UtcNow;
        var order = OrderWithItem(now);
        order.Cancel(now);

        var result = order.Pay(now);

        result.IsFailure.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Cancelled);
    }

    [Fact]
    public void Cancel_FromPending_SucceedsAndRaisesEvent()
    {
        var now = DateTimeOffset.UtcNow;
        var order = OrderWithItem(now);

        var result = order.Cancel(now);

        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Cancelled);
        order.DomainEvents.Should().ContainSingle(e => e is OrderCancelled);
    }

    [Fact]
    public void Cancel_Twice_IsIdempotent()
    {
        var now = DateTimeOffset.UtcNow;
        var order = OrderWithItem(now);
        order.Cancel(now);
        order.ClearDomainEvents();

        var result = order.Cancel(now);

        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Cancelled);
    }

    [Theory]
    [InlineData(OrderStatus.Shipping)]
    [InlineData(OrderStatus.Delivered)]
    public void Cancel_FromShippedOrDelivered_Fails(OrderStatus status)
    {
        var now = DateTimeOffset.UtcNow;
        var order = OrderInStatus(status, now);

        var result = order.Cancel(now);

        result.IsFailure.Should().BeTrue();
        order.Status.Should().Be(status);
    }

    [Fact]
    public void Fulfilment_HappyPath_ReachesDelivered()
    {
        var now = DateTimeOffset.UtcNow;
        var order = OrderWithItem(now);
        order.Pay(now);

        order.StartPackaging();
        order.Status.Should().Be(OrderStatus.Packaging);

        order.Ship();
        order.Status.Should().Be(OrderStatus.Shipping);

        order.Deliver();
        order.Status.Should().Be(OrderStatus.Delivered);
    }

    [Fact]
    public void StartPackaging_FromPending_Throws()
    {
        var now = DateTimeOffset.UtcNow;
        var order = OrderWithItem(now);

        var act = order.StartPackaging;

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Ship_FromPaid_Throws()
    {
        var now = DateTimeOffset.UtcNow;
        var order = OrderInStatus(OrderStatus.Paid, now);

        var act = order.Ship;

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Deliver_FromPending_Throws()
    {
        var now = DateTimeOffset.UtcNow;
        var order = OrderWithItem(now);

        var act = order.Deliver;

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void StartPackaging_Twice_IsIdempotent()
    {
        var now = DateTimeOffset.UtcNow;
        var order = OrderInStatus(OrderStatus.Packaging, now);

        var act = order.StartPackaging;

        act.Should().NotThrow();
        order.Status.Should().Be(OrderStatus.Packaging);
    }

    [Fact]
    public void Deliver_Twice_IsIdempotent()
    {
        var now = DateTimeOffset.UtcNow;
        var order = OrderInStatus(OrderStatus.Delivered, now);

        var act = order.Deliver;

        act.Should().NotThrow();
        order.Status.Should().Be(OrderStatus.Delivered);
    }
}