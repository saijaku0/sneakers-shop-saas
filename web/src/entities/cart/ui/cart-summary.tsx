interface CartSummaryProps {
  itemCount: number;
  subtotal: number;
  total: number;
}

export function CartSummary({ itemCount, subtotal, total }: CartSummaryProps) {
  return (
    <div className="mb-4 space-y-1.5 px-4">
      <div className="flex items-center justify-between text-sm text-muted-foreground">
        <span>Items ({itemCount})</span>
        <span>${subtotal.toFixed(2)}</span>
      </div>
      <div className="flex items-center justify-between text-lg font-bold">
        <span>Total</span>
        <span>${total.toFixed(2)}</span>
      </div>
    </div>
  );
}
