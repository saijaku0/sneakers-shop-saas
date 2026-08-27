import { cn } from "@/shared/lib";
import { selectSubtotal, type CartItem } from "@/entities/cart";

interface OrderSummaryProps {
  items: CartItem[];
  className?: string;
}

export function OrderSummary({ items, className }: OrderSummaryProps) {
  const subtotal = selectSubtotal(items);

  return (
    <div className={cn("rounded-md border bg-card p-5", className)}>
      <h3 className="text-sm font-semibold uppercase tracking-wide">
        Order Summary
      </h3>

      {items.length === 0 ? (
        <p className="mt-4 text-sm text-muted-foreground">Your cart is empty</p>
      ) : (
        <div className="mt-4 flex flex-col gap-4">
          {items.map((item) => (
            <div key={item.warehouseItemId} className="flex items-start gap-3">
              <div className="relative h-14 w-14 shrink-0 overflow-hidden rounded-md border bg-muted">
                <img
                  src={item.previewImageUrl}
                  alt={`${item.brandName} ${item.model}`}
                  className="h-full w-full object-cover object-center"
                />
                <span className="absolute -top-1.5 -right-1.5 flex h-4.5 min-w-4.5 items-center justify-center rounded-full bg-foreground px-1 text-[10px] font-semibold text-background">
                  {item.quantity}
                </span>
              </div>

              <div className="flex min-w-0 flex-1 flex-col">
                <span className="text-xs uppercase tracking-wider text-muted-foreground">
                  {item.brandName}
                </span>
                <span className="line-clamp-1 text-sm font-medium">
                  {item.model}
                </span>
                <span className="text-xs text-muted-foreground">
                  Size: {item.sizeCm} · {item.color}
                </span>
              </div>

              <span className="shrink-0 text-sm font-medium">
                €{(item.unitPrice * item.quantity).toFixed(2)}
              </span>
            </div>
          ))}
        </div>
      )}

      <div className="mt-5 flex flex-col gap-2 border-t pt-4 text-sm">
        <div className="flex items-center justify-between text-muted-foreground">
          <span>Subtotal</span>
          <span>€{subtotal.toFixed(2)}</span>
        </div>
        <div className="flex items-center justify-between text-muted-foreground">
          <span>Shipping</span>
          <span>Free</span>
        </div>
      </div>

      <div className="mt-4 flex items-center justify-between border-t pt-4">
        <span className="text-base font-semibold">Total</span>
        <span className="text-base font-semibold">€{subtotal.toFixed(2)}</span>
      </div>
    </div>
  );
}
