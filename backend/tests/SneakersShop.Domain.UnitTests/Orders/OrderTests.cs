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

    private static OrderItem CreateOrderItem(int quantity = 1, decimal unitPrice = 100m)
        => OrderItem.Create(Guid.CreateVersion7(), quantity, unitPrice, unitPrice * quantity * DiscountRate);

    private static Address CreateAddress()
        => new("Germany", null, "Hof", "Main St", "1", "95028");

    private static Order CreateOrder(DateTimeOffset now, params OrderItem[] items)
        => Order.Create(Guid.CreateVersion7(), CreateAddress(), PaymentMethod.CreditCard,
                        items.Length == 0 ? [CreateOrderItem()] : [.. items], now);

    private static Order OrderInStatus(OrderStatus status, DateTimeOffset now)
    {
        var order = CreateOrder(now, CreateOrderItem());

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
    public void Create_WithValidData_ReturnsPendingOrder()
    {
        var now = DateTimeOffset.UtcNow;
        var item = CreateOrderItem(quantity: 2, unitPrice: 100m);

        var order = CreateOrder(now, item);

        order.Status.Should().Be(OrderStatus.Pending);
        order.PaymentDeadline.Should().Be(now.AddMinutes(30));
        order.PaymentMethod.Should().Be(PaymentMethod.CreditCard);
        order.ShippingAddress.Should().NotBeNull();
        order.OrderItems.Should().ContainSingle();
        order.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Create_SumsItemTotalsIncludingDiscount()
    {
        var now = DateTimeOffset.UtcNow;
        // 2 * 100 = 200, discount 10% = 20, result 180
        var item = CreateOrderItem(quantity: 2, unitPrice: 100m);

        var order = CreateOrder(now, item);

        order.TotalAmount.Should().Be(180m);
    }

    [Fact]
    public void Create_WithMultipleItems_SumsAll()
    {
        var now = DateTimeOffset.UtcNow;
        var a = CreateOrderItem(quantity: 1, unitPrice: 100m); // 90
        var b = CreateOrderItem(quantity: 2, unitPrice: 50m);  // 90

        var order = CreateOrder(now, a, b);

        order.TotalAmount.Should().Be(180m);
    }

    [Fact]
    public void Create_WithEmptyUserId_Throws()
    {
        var now = DateTimeOffset.UtcNow;
        var act = () => Order.Create(Guid.Empty, CreateAddress(), PaymentMethod.CreditCard,
                                     [CreateOrderItem()], now);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithNullAddress_Throws()
    {
        var now = DateTimeOffset.UtcNow;
        var act = () => Order.Create(Guid.CreateVersion7(), null!, PaymentMethod.CreditCard,
                                     [CreateOrderItem()], now);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithNoItems_Throws()
    {
        var now = DateTimeOffset.UtcNow;
        var act = () => Order.Create(Guid.CreateVersion7(), CreateAddress(), PaymentMethod.CreditCard,
                                     [], now);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithUndefinedPaymentMethod_Throws()
    {
        var now = DateTimeOffset.UtcNow;
        var act = () => Order.Create(Guid.CreateVersion7(), CreateAddress(),
                                     (PaymentMethod)999, [CreateOrderItem()], now);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Pay_FromPending_BeforeDeadline_SucceedsAndRaisesEvent()
    {
        var now = DateTimeOffset.UtcNow;
        var order = CreateOrder(now, CreateOrderItem());

        var result = order.Pay(now.AddMinutes(5));

        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Paid);
        order.DomainEvents.Should().ContainSingle(e => e is OrderPaid);
    }

    [Fact]
    public void Pay_AfterDeadline_Fails()
    {
        var now = DateTimeOffset.UtcNow;
        var order = CreateOrder(now, CreateOrderItem());

        var result = order.Pay(now.AddMinutes(31));

        result.IsFailure.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Pending);
    }

    [Fact]
    public void Pay_Twice_IsIdempotent()
    {
        var now = DateTimeOffset.UtcNow;
        var order = CreateOrder(now, CreateOrderItem());
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
        var order = CreateOrder(now, CreateOrderItem());
        order.Cancel(now);

        var result = order.Pay(now);

        result.IsFailure.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Cancelled);
    }

    [Fact]
    public void Cancel_FromPending_SucceedsAndRaisesEvent()
    {
        var now = DateTimeOffset.UtcNow;
        var order = CreateOrder(now, CreateOrderItem());

        var result = order.Cancel(now);

        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Cancelled);
        order.DomainEvents.Should().ContainSingle(e => e is OrderCancelled);
    }

    [Fact]
    public void Cancel_Twice_IsIdempotent()
    {
        var now = DateTimeOffset.UtcNow;
        var order = CreateOrder(now, CreateOrderItem());
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
        var order = CreateOrder(now, CreateOrderItem());
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
        var order = CreateOrder(now, CreateOrderItem());

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
        var order = CreateOrder(now, CreateOrderItem());

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