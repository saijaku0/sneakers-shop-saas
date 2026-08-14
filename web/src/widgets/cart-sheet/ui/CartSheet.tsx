"use client";

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
} from "@/shared/ui";
import {
  selectItemCount,
  selectSubtotal,
  selectHasOutOfStock,
  selectCanCheckout,
  CartButton,
  CartItemRow,
  CartSummary,
  useCart,
} from "@/entities/cart";

export function CartSheet() {
  const { items, changeQuantity, removeItem } = useCart();

  const itemCount = selectItemCount(items);
  const subtotal = selectSubtotal(items);
  const canCheckout = selectCanCheckout(items);
  const hasOutOfStock = selectHasOutOfStock(items);

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
          {items.length > 0 ? (
            <div className="flex flex-col gap-6">
              {items.map((item) => (
                <CartItemRow
                  key={item.warehouseItemId}
                  item={item}
                  onQuantityChange={changeQuantity}
                  onRemove={removeItem}
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
            <Button className="w-full" size="lg" disabled={!canCheckout}>
              Checkout
            </Button>
            {hasOutOfStock && (
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
