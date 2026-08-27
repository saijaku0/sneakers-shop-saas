import Link from "next/link";
import { CheckCircle2, MapPin, Wallet } from "lucide-react";
import { Button } from "@/shared/ui";
import { formatDateTime } from "@/shared/lib";
import {
  formatPaymentMethodName,
  OrderStatusBadge,
  OrderItemsList,
  type OrderDetail,
} from "@/entities/order";

export function OrderConfirmation({ order }: { order: OrderDetail }) {
  const { shippingAddress } = order;

  return (
    <div className="mx-auto w-full max-w-160 px-4 py-10 sm:px-6">
      <div className="mb-8 flex flex-col items-center gap-3 text-center">
        <CheckCircle2 className="h-10 w-10 text-primary" />
        <h1 className="font-display text-2xl font-bold uppercase tracking-tight text-foreground">
          Order Placed
        </h1>
        <p className="text-sm text-muted-foreground">
          Thank you for your purchase. Here are your order details.
        </p>
      </div>

      <div className="flex flex-col gap-4">
        <div className="flex items-center justify-between rounded-md border bg-card p-4">
          <div className="flex flex-col">
            <span className="text-xs text-muted-foreground">Order number</span>
            <span className="font-mono text-sm font-semibold">{order.id}</span>
            <span className="mt-1 text-xs text-muted-foreground">
              Placed on {formatDateTime(order.createdAt)}
            </span>
          </div>
          <OrderStatusBadge status={order.status} />
        </div>

        <OrderItemsList items={order.items} />

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

        <div className="flex items-center justify-between rounded-md border bg-card p-4">
          <span className="text-base font-semibold">Total</span>
          <span className="text-base font-semibold">
            €{order.totalAmount.toFixed(2)}
          </span>
        </div>

        <Button size="lg" className="mt-2 w-full" asChild>
          <Link href="/">Continue Shopping</Link>
        </Button>
      </div>
    </div>
  );
}
