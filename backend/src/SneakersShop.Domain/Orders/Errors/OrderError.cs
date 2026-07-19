using SneakersShop.Domain.Common.Results;
using SneakersShop.Domain.Orders.Enums;

namespace SneakersShop.Domain.Orders.Errors;

public static class OrderError
{
    public static Error InvalidStateTransition(OrderStatus currentStatus, OrderStatus attemptedStatus) =>
        Error.Invalid("Order.InvalidStateTransition", $"Cannot transition order from {currentStatus} to {attemptedStatus}.");
    public static Error PaymentDeadlineExpired(DateTimeOffset paymentDeadline) =>
        Error.Invalid("Order.PaymentDeadlineExpired", $"Payment deadline of {paymentDeadline} has expired.");
}