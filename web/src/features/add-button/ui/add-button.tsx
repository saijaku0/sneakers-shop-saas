"use client";

import { ProductListItem } from "@/entities/product/model/types";
import { Button } from "@/shared/ui";

interface AddToCartButtonProps {
  product: ProductListItem;
}

export function AddCardButton({ product }: AddToCartButtonProps) {
  return (
    <Button
      onClick={() => {
        console.log("Add to cart:", product.id);
      }}
    >
      Add to cart
    </Button>
  );
}
