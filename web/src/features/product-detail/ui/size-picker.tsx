"use client";

import type { ProductSize } from "@/entities/product";
import { Button } from "@/shared/ui";

export function SizePicker({
  sizes,
  selected,
  onSelect,
}: {
  sizes: ProductSize[];
  selected: ProductSize | null;
  onSelect: (sizeCm: ProductSize) => void;
}) {
  if (sizes.length === 0) {
    return <p className="text-sm text-muted-foreground">No sizes available</p>;
  }

  return (
    <div className="flex flex-col gap-3">
      <div className="flex items-center justify-between">
        <p className="text-sm font-medium text-foreground">Size (cm)</p>
        <Button
          variant="link"
          className="text-sm text-muted-foreground underline hover:text-foreground"
        >
          Size guide
        </Button>
      </div>

      <div className="grid grid-cols-5 gap-1">
        {sizes.map((s) => {
          const isSelected = selected?.warehouseItemId === s.warehouseItemId;
          return (
            <button
              key={s.warehouseItemId}
              type="button"
              disabled={!s.inStock}
              onClick={() => onSelect(s)}
              aria-pressed={isSelected}
              className={`h-11 border text-sm transition-colors
                ${
                  isSelected
                    ? "border-foreground bg-foreground text-background"
                    : "border-border"
                }
                ${
                  s.inStock
                    ? "cursor-pointer hover:border-foreground"
                    : "cursor-not-allowed text-muted-foreground line-through opacity-40"
                }`}
            >
              {s.sizeCm}
            </button>
          );
        })}
      </div>
    </div>
  );
}
