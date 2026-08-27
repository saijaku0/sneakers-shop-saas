import Link from "next/link";
import { ArrowLeft, MapPin, Wallet } from "lucide-react";
import { formatDateTime } from "@/shared/lib";
import {
  formatOrderId,
  formatPaymentMethodName,
  OrderStatusBadge,
  OrderItemsList,
  type OrderDetail,
} from "@/entities/order";

export function OrderDetails({ order }: { order: OrderDetail }) {
  const { shippingAddress } = order;

  return (
    <div className="flex flex-col gap-6 py-8">
      <Link
        href="/profile/orders"
        className="inline-flex w-fit items-center gap-1.5 text-xs font-medium uppercase tracking-wide text-muted-foreground transition-colors hover:text-foreground"
      >
        <ArrowLeft className="size-3.5" />
        Order history
      </Link>

      <div className="flex flex-wrap items-start justify-between gap-4">
        <div className="flex flex-col gap-1">
          <h1 className="font-display text-2xl font-bold uppercase tracking-tight text-foreground">
            Order {formatOrderId(order.id)}
          </h1>
          <p className="text-sm text-muted-foreground">
            Placed on {formatDateTime(order.createdAt)}
          </p>
        </div>
        <OrderStatusBadge status={order.status} className="h-7 px-3 text-sm" />
      </div>

      <OrderItemsList items={order.items} />

      <div className="grid gap-4 sm:grid-cols-2">
        <div className="rounded-md border bg-card p-4">
          <div className="flex items-center gap-2 text-sm font-semibold">
            <MapPin className="h-4 w-4" />
            Shipping Address
          </div>
          <div className="mt-3 text-sm text-muted-foreground">
            <p className="text-foreground">
              {shippingAddress.street} {shippingAddress.houseNumber}
            </p>
            <p>
              {shippingAddress.zipCode} {shippingAddress.city}
              {shippingAddress.state ? `, ${shippingAddress.state}` : ""}
            </p>
            <p>{shippingAddress.country}</p>
          </div>
        </div>

        <div className="rounded-md border bg-card p-4">
          <div className="flex items-center gap-2 text-sm font-semibold">
            <Wallet className="h-4 w-4" />
            Payment
          </div>
          <div className="mt-3 flex flex-col gap-1 text-sm text-muted-foreground">
            <div className="flex items-center justify-between">
              <span>Method</span>
              <span className="text-foreground">
                {formatPaymentMethodName(order.paymentMethod)}
              </span>
            </div>
            <div className="flex items-center justify-between">
              <span>Payment deadline</span>
              <span className="text-foreground">
                {formatDateTime(order.paymentDeadline)}
              </span>
            </div>
          </div>
        </div>
      </div>

      <div className="flex items-center justify-between rounded-md border bg-card p-4">
        <span className="text-base font-semibold">Total</span>
        <span className="font-display text-xl font-bold text-foreground">
          €{order.totalAmount.toFixed(2)}
        </span>
      </div>
    </div>
  );
}
