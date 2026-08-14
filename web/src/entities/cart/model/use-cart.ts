"use client";

import { useState } from "react";
import type { CartItem } from "./types";
import { mockCart } from "../api/mock-cart";

const clampQty = (qty: number, max: number) => Math.max(1, Math.min(qty, max));

export function useCart() {
  const [items, setItems] = useState<CartItem[]>(mockCart.items);

  const addItem = (item: CartItem) =>
    setItems((prev) => {
      const existing = prev.find(
        (i) => i.warehouseItemId === item.warehouseItemId,
      );
      if (existing) {
        return prev.map((i) =>
          i.warehouseItemId === item.warehouseItemId
            ? {
                ...i,
                quantity: clampQty(i.quantity + item.quantity, i.maxAvailable),
              }
            : i,
        );
      }
      return [...prev, item];
    });

  const changeQuantity = (id: string, delta: number) =>
    setItems((prev) =>
      prev.map((i) =>
        i.warehouseItemId === id && i.inStock
          ? { ...i, quantity: clampQty(i.quantity + delta, i.maxAvailable) }
          : i,
      ),
    );

  const removeItem = (id: string) =>
    setItems((prev) => prev.filter((i) => i.warehouseItemId !== id));

  const clear = () => setItems([]);

  return { items, addItem, changeQuantity, removeItem, clear };
}
