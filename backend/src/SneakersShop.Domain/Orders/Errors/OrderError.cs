using SneakersShop.Domain.Common.Results;
using SneakersShop.Domain.Orders.Enums;

namespace SneakersShop.Domain.Orders.Errors;

public static class OrderError
{
    public static Error InvalidStateTransition(OrderStatus currentStatus, OrderStatus attemptedStatus) =>
        Error.Invalid("Order.InvalidStateTransition", $"Cannot transition order from {currentStatus} to {attemptedStatus}.");
    public static Error PaymentDeadlineExpired(DateTimeOffset paymentDeadline) =>
        Error.Invalid("Order.PaymentDeadlineExpired", $"Payment deadline of {paymentDeadline} has expired.");
    public static Error ShippingAddressRequired =>
        Error.Validation("ShippingAddress.Required", "A shipping address is required to proceed.");
    public static Error DuplicateItem =>
        Error.Conflict("Order.DuplicateItem", "This item is already in the order.");
    public static Error CannotModify(OrderStatus status) =>
        Error.Conflict("Order.CannotModify", $"Cannot add items. Order is in '{status}' state.");
}