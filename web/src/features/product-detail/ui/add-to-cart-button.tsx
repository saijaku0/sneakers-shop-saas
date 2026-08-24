"use client";

import { Button } from "@/shared/ui";
import type { ProductDetailVariant, ProductSize } from "@/entities/product";

export function AddToCartButton({
  variant,
  selectedSize,
}: {
  variant: ProductDetailVariant;
  selectedSize: ProductSize | null;
}) {
  const handleAdd = () => {
    if (!selectedSize) return;

    console.log("add to cart payload:", {
      variantId: variant.variantId,
      warehouseItemId: selectedSize.warehouseItemId,
      sizeCm: selectedSize.sizeCm,
      quantity: 1
    });
  };

  return (
    <Button
      size="lg"
      className="w-full"
      disabled={!selectedSize}
      onClick={handleAdd}
    >
      {!selectedSize ? "Select a size" : "Add to cart"}
    </Button>
  );
}
