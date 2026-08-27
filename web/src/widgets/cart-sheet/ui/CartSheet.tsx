"use client";

import { useSelector, useDispatch } from "react-redux";

import {
  Sheet,
  SheetClose,
  SheetContent,
  SheetDescription,
  SheetFooter,
  SheetHeader,
  SheetTitle,
  SheetTrigger,
  Button,
  Skeleton,
} from "@/shared/ui";
import {
  selectItemCount,
  selectSubtotal,
  selectHasOutOfStock,
  selectCanCheckout,
  CartButton,
  CartItemRow,
  CartSummary,
  selectCartItems,
  selectCartSyncStatus,
  clampCartQuantity,
  useGetCartQuery,
  useRemoveFromCartMutation,
  useUpdateCartItemQuantityMutation,
  changeQuantity,
  setCartItemQuantity,
  setCartFromServer,
  removeFromCart as removeFromLocalCart,
} from "@/entities/cart";
import { selectToken } from "@/entities/session";
import Link from "next/link";

export function CartSheet() {
  const items = useSelector(selectCartItems);
  const token = useSelector(selectToken);
  const syncStatus = useSelector(selectCartSyncStatus);
  const isAuthenticated = Boolean(token);
  const dispatch = useDispatch();

  const [removeFromServerCart] = useRemoveFromCartMutation();
  const [updateQuantity] = useUpdateCartItemQuantityMutation();

  const { isLoading } = useGetCartQuery(undefined, {
    skip: !isAuthenticated || syncStatus === "syncing",
  });

  const itemCount = selectItemCount(items);
  const subtotal = selectSubtotal(items);
  const canCheckout = selectCanCheckout(items);
  const hasOutOfStock = selectHasOutOfStock(items);

  const handleQuantityChange = async (id: string, delta: number) => {
    if (!isAuthenticated) {
      dispatch(changeQuantity({ id, delta }));
      return;
    }

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

  const handleRemove = async (warehouseItemId: string) => {
    if (!isAuthenticated) {
      dispatch(removeFromLocalCart(warehouseItemId));
      return;
    }

    const snapshot = items;
    dispatch(removeFromLocalCart(warehouseItemId));

    try {
      await removeFromServerCart(warehouseItemId).unwrap();
    } catch (err) {
      dispatch(setCartFromServer(snapshot));
      console.error("Failed to remove item from cart:", err);
    }
  };

  return (
    <Sheet>
      <SheetTrigger asChild>
        <CartButton count={itemCount} />
      </SheetTrigger>

      <SheetContent className="flex w-full flex-col sm:max-w-100 xl:max-w-120 2xl:max-w-160">
        <SheetHeader>
          <SheetTitle>Cart</SheetTitle>
          <SheetDescription>
            Review your items before checkout.
          </SheetDescription>
        </SheetHeader>

        <div className="flex-1 overflow-y-auto px-4 py-4">
          {isLoading ? (
            <div className="flex flex-col gap-6">
              {Array.from({ length: 3 }).map((_, index) => (
                <CartItemSkeleton key={index} />
              ))}
            </div>
          ) : items.length > 0 ? (
            <div className="flex flex-col gap-6">
              {items.map((item) => (
                <CartItemRow
                  key={item.warehouseItemId}
                  item={item}
                  onQuantityChange={handleQuantityChange}
                  onRemove={handleRemove}
                />
              ))}
            </div>
          ) : (
            <div className="flex h-full items-center justify-center text-muted-foreground">
              Your cart is empty
            </div>
          )}
        </div>

        <div className="mt-auto border-t pt-4">
          <CartSummary
            itemCount={itemCount}
            subtotal={subtotal}
            total={subtotal}
          />

          <SheetFooter className="flex-col gap-2 sm:flex-col">
            {isAuthenticated ? (
              <Link href="/checkout" className="w-full">
                <Button
                  className="w-full"
                  size="lg"
                  disabled={isLoading || !canCheckout}
                >
                  Checkout
                </Button>
              </Link>
            ) : (
              <Button className="w-full" size="lg" disabled>
                Sign in to checkout
              </Button>
            )}

            {!isLoading && hasOutOfStock && (
              <p className="text-center text-[10px] text-destructive">
                Remove out of stock items to checkout
              </p>
            )}

            <SheetClose asChild>
              <Button variant="outline" className="w-full">
                Continue shopping
              </Button>
            </SheetClose>
          </SheetFooter>
        </div>
      </SheetContent>
    </Sheet>
  );
}

function CartItemSkeleton() {
  return (
    <div className="flex gap-4">
      <Skeleton className="h-24 w-24 shrink-0 rounded-md" />

      <div className="flex min-w-0 flex-1 flex-col gap-2">
        <Skeleton className="h-4 w-3/4" />
        <Skeleton className="h-3 w-1/2" />
        <Skeleton className="h-3 w-1/3" />

        <div className="mt-auto flex items-center justify-between">
          <Skeleton className="h-8 w-24" />
          <Skeleton className="h-5 w-16" />
        </div>
      </div>
    </div>
  );
}
