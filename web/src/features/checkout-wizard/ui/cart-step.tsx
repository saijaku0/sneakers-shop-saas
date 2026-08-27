"use client";

import { Minus, Plus, Trash2 } from "lucide-react";
import { useDispatch, useSelector } from "react-redux";
import { Button, Skeleton } from "@/shared/ui";
import {
  clampCartQuantity,
  removeFromCart as removeFromLocalCart,
  selectCartItems,
  setCartFromServer,
  setCartItemQuantity,
  useGetCartQuery,
  useRemoveFromCartMutation,
  useUpdateCartItemQuantityMutation,
} from "@/entities/cart";

export function CartStep() {
  const dispatch = useDispatch();
  const items = useSelector(selectCartItems);
  const { isLoading, isError } = useGetCartQuery();

  const [updateQuantity] = useUpdateCartItemQuantityMutation();
  const [removeItem] = useRemoveFromCartMutation();

  const handleQuantityChange = async (id: string, delta: number) => {
    const item = items.find((i) => i.warehouseItemId === id);
    if (!item || !item.isAvailable) return;

    const previousQuantity = item.quantity;
    const nextQuantity = clampCartQuantity(
      previousQuantity + delta,
      item.available,
    );
    if (nextQuantity === previousQuantity) return;

    dispatch(setCartItemQuantity({ id, quantity: nextQuantity }));

    try {
      await updateQuantity({
        warehouseItemId: id,
        quantity: nextQuantity,
      }).unwrap();
    } catch (err) {
      dispatch(setCartItemQuantity({ id, quantity: previousQuantity }));
      console.error("Failed to update cart item quantity:", err);
    }
  };

  const handleRemove = async (id: string) => {
    const snapshot = items;
    dispatch(removeFromLocalCart(id));

    try {
      await removeItem(id).unwrap();
    } catch (err) {
      dispatch(setCartFromServer(snapshot));
      console.error("Failed to remove item from cart:", err);
    }
  };

  if (isLoading) {
    return (
      <div className="flex flex-col gap-3">
        {Array.from({ length: 3 }).map((_, index) => (
          <div
            key={index}
            className="flex items-center gap-4 rounded-md border bg-card p-4"
          >
            <Skeleton className="h-20 w-20 shrink-0 rounded-md" />
            <div className="flex flex-1 flex-col gap-2">
              <Skeleton className="h-3 w-20" />
              <Skeleton className="h-4 w-40" />
              <Skeleton className="h-3 w-28" />
            </div>
          </div>
        ))}
      </div>
    );
  }

  if (isError) {
    return (
      <div className="flex h-40 items-center justify-center rounded-md border text-sm text-muted-foreground">
        Couldn&apos;t load your cart. Please try again.
      </div>
    );
  }

  if (items.length === 0) {
    return (
      <div className="flex h-40 items-center justify-center rounded-md border text-sm text-muted-foreground">
        Your cart is empty
      </div>
    );
  }

  return (
    <div className="flex flex-col gap-3">
      {items.map((item) => (
        <div
          key={item.warehouseItemId}
          className="flex flex-col gap-4 rounded-md border bg-card p-4 sm:flex-row sm:items-center"
        >
          <div className="flex gap-4">
            <div className="h-20 w-20 shrink-0 overflow-hidden rounded-md border bg-muted">
              <img
                src={item.previewImageUrl}
                alt={`${item.brandName} ${item.model}`}
                className="h-full w-full object-cover object-center"
              />
            </div>

            <div className="flex min-w-0 flex-1 flex-col gap-1 sm:hidden">
              <span className="text-xs uppercase tracking-wider text-muted-foreground">
                {item.brandName}
              </span>
              <span className="line-clamp-1 text-sm font-semibold">
                {item.model}
              </span>
              <span className="text-xs text-muted-foreground">
                Size: {item.sizeCm} · Color: {item.color}
              </span>
              <span className="mt-1 text-sm font-semibold">
                €{(item.unitPrice * item.quantity).toFixed(2)}
              </span>
            </div>
          </div>

          <div className="hidden min-w-0 flex-1 flex-col gap-1 sm:flex">
            <span className="text-xs uppercase tracking-wider text-muted-foreground">
              {item.brandName}
            </span>
            <span className="line-clamp-1 text-sm font-semibold">
              {item.model}
            </span>
            <span className="text-xs text-muted-foreground">
              Size: {item.sizeCm} · Color: {item.color}
            </span>
          </div>

          <div className="flex items-center justify-between gap-4 sm:justify-end">
            <div className="flex shrink-0 items-center rounded-md border">
              <Button
                variant="ghost"
                size="icon"
                className="h-8 w-8 rounded-none"
                onClick={() => handleQuantityChange(item.warehouseItemId, -1)}
                disabled={!item.isAvailable || item.quantity <= 1}
              >
                <Minus className="h-3 w-3" />
              </Button>
              <span className="w-8 text-center text-sm font-medium">
                {item.quantity}
              </span>
              <Button
                variant="ghost"
                size="icon"
                className="h-8 w-8 rounded-none"
                onClick={() => handleQuantityChange(item.warehouseItemId, 1)}
                disabled={
                  !item.isAvailable ||
                  (item.available !== undefined &&
                    item.quantity >= item.available)
                }
              >
                <Plus className="h-3 w-3" />
              </Button>
            </div>

            <span className="hidden w-20 shrink-0 text-right text-sm font-semibold sm:block">
              €{(item.unitPrice * item.quantity).toFixed(2)}
            </span>

            <Button
              variant="ghost"
              size="icon"
              className="h-8 w-8 shrink-0 text-muted-foreground hover:text-destructive"
              onClick={() => handleRemove(item.warehouseItemId)}
            >
              <Trash2 className="h-4 w-4" />
            </Button>
          </div>
        </div>
      ))}
    </div>
  );
}
