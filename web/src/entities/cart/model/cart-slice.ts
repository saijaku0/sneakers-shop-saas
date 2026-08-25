import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { CartItem } from "../lib/types";

export const clampCartQuantity = (qty: number, max?: number) => {
  if (max === undefined) return Math.max(1, qty);
  return Math.max(1, Math.min(qty, max));
};

const CART_KEY = "local_cart";

const loadCartFromStorage = () => {
  try {
    if (typeof window !== "undefined") {
      const data = localStorage.getItem(CART_KEY);
      return data ? JSON.parse(data) : [];
    }
    return [];
  } catch {
    return [];
  }
};

const saveCartToStorage = (items: CartItem[]) => {
  if (typeof window !== "undefined") {
    localStorage.setItem(CART_KEY, JSON.stringify(items));
  }
};

const hasStoredSession = () =>
  typeof window !== "undefined" && Boolean(localStorage.getItem("accessToken"));

export type CartSyncStatus = "idle" | "syncing" | "synced";

export interface CartState {
  items: CartItem[];
  syncStatus: CartSyncStatus;
}

const initialState: CartState = {
  items: hasStoredSession() ? [] : loadCartFromStorage(),
  syncStatus: "idle",
};

export const cartSlice = createSlice({
  name: "cart",
  initialState,
  reducers: {
    addToCart: (state, action) => {
      const existing = state.items.find(
        (item) => item.warehouseItemId === action.payload.warehouseItemId,
      );
      if (existing) {
        existing.quantity = clampCartQuantity(
          existing.quantity + action.payload.quantity,
          existing.available,
        );
      } else {
        state.items.push(action.payload);
      }
      saveCartToStorage(state.items);
    },
    changeQuantity: (
      state,
      action: PayloadAction<{ id: string; delta: number }>,
    ) => {
      const existing = state.items.find(
        (item) => item.warehouseItemId === action.payload.id,
      );

      if (existing && existing.isAvailable) {
        existing.quantity = clampCartQuantity(
          existing.quantity + action.payload.delta,
          existing.available,
        );
        saveCartToStorage(state.items);
      }
    },
    removeFromCart: (state, action) => {
      state.items = state.items.filter(
        (item) => item.warehouseItemId !== action.payload,
      );
      saveCartToStorage(state.items);
    },
    clearCart: (state) => {
      state.items = [];
      state.syncStatus = "idle";
      if (typeof window !== "undefined") {
        localStorage.removeItem(CART_KEY);
      }
    },
    setCartFromServer: (state, action: PayloadAction<CartItem[]>) => {
      state.items = action.payload;
    },
    setCartItemQuantity: (
      state,
      action: PayloadAction<{ id: string; quantity: number }>,
    ) => {
      const existing = state.items.find(
        (item) => item.warehouseItemId === action.payload.id,
      );
      if (existing) {
        existing.quantity = clampCartQuantity(
          action.payload.quantity,
          existing.available,
        );
      }
    },
    clearLocalCart: () => {
      if (typeof window !== "undefined") {
        localStorage.removeItem(CART_KEY);
      }
    },
    startCartSync: (state) => {
      state.syncStatus = "syncing";
    },
    finishCartSync: (state) => {
      state.syncStatus = "synced";
    },
  },
});

export const {
  addToCart,
  changeQuantity,
  removeFromCart,
  clearCart,
  clearLocalCart,
  setCartFromServer,
  setCartItemQuantity,
  startCartSync,
  finishCartSync,
} = cartSlice.actions;
export const cartReducer = cartSlice.reducer;
export default cartSlice.reducer;
