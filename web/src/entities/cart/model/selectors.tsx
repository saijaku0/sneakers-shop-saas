import { RootState } from "@/app/store";
import type { CartItem } from "../lib/types";

export const selectItemCount = (items: CartItem[]) =>
  items.reduce((sum, i) => sum + i.quantity, 0);

export const selectSubtotal = (items: CartItem[]) =>
  items
    .filter((i) => i.isAvailable)
    .reduce((sum, i) => sum + i.unitPrice * i.quantity, 0);

export const selectHasOutOfStock = (items: CartItem[]) =>
  items.some((i) => !i.isAvailable);

export const selectCanCheckout = (items: CartItem[]) =>
  items.length > 0 && !selectHasOutOfStock(items) && selectSubtotal(items) > 0;

export const selectCartItems = (state: RootState) => state.cart.items;

export const selectCartSyncStatus = (state: RootState) => state.cart.syncStatus;
