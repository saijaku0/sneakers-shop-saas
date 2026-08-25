"use client";

import { useDispatch, useSelector } from "react-redux";

import { Button } from "@/shared/ui";
import type {
  ProductDetail,
  ProductDetailVariant,
  ProductSize,
} from "@/entities/product";

import { addToCart, useAddToCartMutation } from "@/entities/cart";

import { selectToken } from "@/entities/session";

interface AddToCartButtonProps {
  product: ProductDetail;
  variant: ProductDetailVariant;
  selectedSize: ProductSize | null;
}

export function AddToCartButton({
  product,
  variant,
  selectedSize,
}: AddToCartButtonProps) {
  const dispatch = useDispatch();

  const token = useSelector(selectToken);
  const isAuthenticated = Boolean(token);

  const [addToCartRequest, { isLoading }] = useAddToCartMutation();

  const handleAddToCart = async () => {
    if (!selectedSize) return;

    if (!isAuthenticated) {
      dispatch(
        addToCart({
          warehouseItemId: selectedSize.warehouseItemId,
          productId: product.id,
          model: product.model,
          brandName: product.brandName,
          sizeCm: selectedSize.sizeCm,
          color: variant.colorName,
          unitPrice: product.basePrice,
          quantity: 1,
          previewImageUrl: variant.images[0] ?? "",
          isAvailable: selectedSize.inStock,
        }),
      );

      return;
    }
    try {
      await addToCartRequest({
        warehouseItemId: selectedSize.warehouseItemId,
        quantity: 1,
      }).unwrap();
    } catch (error) {
      console.error("Failed to add item to cart:", error);
    }
  };

  return (
    <Button
      size="lg"
      className="w-full"
      disabled={!selectedSize || isLoading}
      onClick={handleAddToCart}
    >
      {isLoading
        ? "Adding..."
        : !selectedSize
          ? "Select a size"
          : "Add to cart"}
    </Button>
  );
}
