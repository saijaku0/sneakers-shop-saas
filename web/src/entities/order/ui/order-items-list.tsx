/* eslint-disable @next/next/no-img-element */
import type { OrderItem } from "../lib/types";

export function OrderItemsList({ items }: { items: OrderItem[] }) {
  return (
    <div className="rounded-md border bg-card p-4">
      <div className="text-sm font-semibold">Items ({items.length})</div>
      <div className="mt-3 flex flex-col gap-4">
        {items.map((item) => (
          <div key={item.warehouseItemId} className="flex items-start gap-4">
            <div className="h-16 w-16 shrink-0 overflow-hidden rounded-md border bg-muted">
              <img
                src={item.previewImageUrl}
                alt={`${item.brandName} ${item.model}`}
                className="h-full w-full object-cover object-center"
              />
            </div>

            <div className="flex min-w-0 flex-1 flex-col">
              <span className="text-xs uppercase tracking-wider text-muted-foreground">
                {item.brandName}
              </span>
              <span className="line-clamp-1 text-sm font-semibold">
                {item.model}
              </span>
              <span className="text-xs text-muted-foreground">
                Size: {item.sizeCm} · Color: {item.color}
              </span>
              <span className="mt-1 text-xs text-muted-foreground">
                {item.quantity} × €{item.unitPrice.toFixed(2)}
                {item.discountAmount > 0 && (
                  <> · −€{item.discountAmount.toFixed(2)} discount</>
                )}
              </span>
            </div>

            <span className="shrink-0 text-sm font-semibold">
              €{item.totalPrice.toFixed(2)}
            </span>
          </div>
        ))}
      </div>
    </div>
  );
}
