import type { CartItem } from "./types";

export const selectItemCount = (items: CartItem[]) =>
  items.reduce((sum, i) => sum + i.quantity, 0);

export const selectSubtotal = (items: CartItem[]) =>
  items
    .filter((i) => i.inStock)
    .reduce((sum, i) => sum + i.unitPrice * i.quantity, 0);

export const selectHasOutOfStock = (items: CartItem[]) =>
  items.some((i) => !i.inStock);

export const selectCanCheckout = (items: CartItem[]) =>
  items.length > 0 && !selectHasOutOfStock(items) && selectSubtotal(items) > 0;
