"use client";

import { Button } from "@/shared/ui";
import type { ProductDetailVariant } from "@/entities/product";

export function AddToCartButton({
  variant,
  sizeCm,
}: {
  variant: ProductDetailVariant;
  sizeCm: number | null;
}) {
  const handleAdd = () => {
    if (sizeCm === null) return;
    // TODO: server-side cart. Once POST /cart/items is available —
    // send { variantId: variant.variantId, sizeCm, quantity: 1 },
    // the backend will return a CartItem with warehouseItemId and maxAvailable.
    console.log("add to cart:", variant.variantId, sizeCm);
  };

  return (
    <Button
      size="lg"
      className="w-full"
      disabled={sizeCm === null}
      onClick={handleAdd}
    >
      {sizeCm === null ? "Select a size" : "Add to cart"}
    </Button>
  );
}
