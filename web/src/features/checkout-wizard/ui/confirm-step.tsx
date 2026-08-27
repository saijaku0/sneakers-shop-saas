import { MapPin, Wallet } from "lucide-react";
import type { CartItem } from "@/entities/cart";
import type { DefaultShippingAddress } from "@/shared/lib";
import { PAYMENT_METHOD_LABELS, type PaymentMethod } from "@/entities/order";

interface ConfirmStepProps {
  items: CartItem[];
  address: DefaultShippingAddress | null;
  paymentMethod: PaymentMethod;
}

export function ConfirmStep({
  items,
  address,
  paymentMethod,
}: ConfirmStepProps) {
  return (
    <div className="flex flex-col gap-4">
      <div className="rounded-md border bg-card p-4">
        <div className="flex items-center gap-2 text-sm font-semibold">
          <MapPin className="h-4 w-4" />
          Shipping Address
        </div>
        {address ? (
          <div className="mt-3 text-sm text-muted-foreground">
            <p className="text-foreground">
              {address.street} {address.houseNumber}
            </p>
            <p>
              {address.zipCode} {address.city}
              {address.state ? `, ${address.state}` : ""}
            </p>
            <p>{address.country}</p>
          </div>
        ) : (
          <p className="mt-3 text-sm text-muted-foreground">
            No shipping address provided
          </p>
        )}
      </div>

      <div className="rounded-md border bg-card p-4">
        <div className="flex items-center gap-2 text-sm font-semibold">
          <Wallet className="h-4 w-4" />
          Payment Method
        </div>
        <p className="mt-1 text-sm text-muted-foreground">
          {PAYMENT_METHOD_LABELS[paymentMethod]}
        </p>
      </div>

      <div className="rounded-md border bg-card p-4">
        <div className="text-sm font-semibold">Items ({items.length})</div>
        <div className="mt-3 flex flex-col gap-3">
          {items.map((item) => (
            <div
              key={item.warehouseItemId}
              className="flex items-center justify-between text-sm text-muted-foreground"
            >
              <span className="text-foreground">
                {item.quantity} × {item.brandName} {item.model}
              </span>
              <span>€{(item.unitPrice * item.quantity).toFixed(2)}</span>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}
