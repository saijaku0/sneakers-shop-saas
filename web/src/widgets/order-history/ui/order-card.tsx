import Link from "next/link";
import { ChevronRight } from "lucide-react";
import { formatDateTime } from "@/shared/lib";
import {
  OrderStatusBadge,
  formatOrderId,
  type OrderSummary,
} from "@/entities/order";
import { OrderPreviewStack } from "./order-preview-stack";

export function OrderCard({ order }: { order: OrderSummary }) {
  return (
    <Link
      href={`/profile/orders/${order.id}`}
      className="group flex items-center gap-4 rounded-md border bg-card p-4 transition-colors hover:border-foreground sm:gap-6 sm:p-5"
    >
      <OrderPreviewStack
        images={order.previewImages}
        alt={order.itemsPreviewText}
      />

      <div className="flex min-w-0 flex-1 flex-col gap-1.5">
        <div className="flex flex-wrap items-center gap-2">
          <span className="font-mono text-xs text-muted-foreground">
            {formatOrderId(order.id)}
          </span>
          <OrderStatusBadge status={order.status} />
        </div>

        <p className="line-clamp-1 text-sm font-semibold text-foreground">
          {order.itemsPreviewText}
        </p>

        <div className="flex flex-wrap items-center gap-x-3 gap-y-1 text-xs text-muted-foreground">
          <span>{formatDateTime(order.createdAt)}</span>
          <span aria-hidden>·</span>
          <span>
            {order.itemCount} {order.itemCount === 1 ? "item" : "items"}
          </span>
        </div>
      </div>

      <div className="flex shrink-0 flex-col items-end gap-2">
        <span className="font-display text-lg font-bold text-foreground">
          €{order.totalAmount.toFixed(2)}
        </span>
        <span className="flex items-center gap-1 text-xs font-medium uppercase tracking-wide text-muted-foreground transition-colors group-hover:text-foreground">
          <span className="hidden sm:inline">View details</span>
          <ChevronRight className="size-3.5 transition-transform group-hover:translate-x-0.5" />
        </span>
      </div>
    </Link>
  );
}
