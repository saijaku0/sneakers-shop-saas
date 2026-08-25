export type {
  Cart,
  CartItem,
  LocalCartItem,
  UpdateCartQuantityRequest,
} from "./lib/types";
export {
  selectItemCount,
  selectSubtotal,
  selectHasOutOfStock,
  selectCanCheckout,
  selectCartItems,
  selectCartSyncStatus,
} from "./model/selectors";
export { CartButton } from "./ui/cart-button";
export { CartItem as CartItemRow } from "./ui/cart-item";
export { CartSummary } from "./ui/cart-summary";

export { cartReducer, clampCartQuantity } from "./model/cart-slice";
export type { CartSyncStatus } from "./model/cart-slice";

export {
  addToCart,
  changeQuantity,
  removeFromCart,
  clearCart,
  clearLocalCart,
  setCartFromServer,
  setCartItemQuantity,
  startCartSync,
  finishCartSync,
} from "./model/cart-slice";

export {
  useGetCartQuery,
  useLazyGetCartQuery,
  useAddToCartMutation,
  useRemoveFromCartMutation,
  useUpdateCartItemQuantityMutation,
  useSyncCartMutation,
} from "./api/cart-api";
