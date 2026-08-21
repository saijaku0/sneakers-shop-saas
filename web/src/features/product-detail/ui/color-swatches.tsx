/* eslint-disable @next/next/no-img-element */
"use client";

import type { ProductDetailVariant } from "@/entities/product";

export function ColorSwatches({
  variants,
  selectedId,
  onSelect,
}: {
  variants: ProductDetailVariant[];
  selectedId: string;
  onSelect: (id: string) => void;
}) {
  if (variants.length <= 1) return null;

  const selectedName = variants.find(
    (v) => v.variantId === selectedId,
  )?.colorName;

  return (
    <div className="flex flex-col gap-3">
      <p className="text-sm font-medium text-foreground">Colors</p>

      <div className="grid grid-cols-5 gap-2">
        {variants.map((v) => (
          <button
            key={v.variantId}
            type="button"
            onClick={() => onSelect(v.variantId)}
            aria-label={v.colorName}
            aria-pressed={v.variantId === selectedId}
            className={`aspect-square overflow-hidden border transition-colors ${
              v.variantId === selectedId
                ? "border-foreground border-b-2"
                : "border-border hover:border-foreground"
            }`}
          >
            <img
              src={v.images[0] || ""}
              alt={v.colorName}
              className="h-full w-full object-cover"
            />
          </button>
        ))}
      </div>

      <p className="text-sm text-muted-foreground">{selectedName}</p>
    </div>
  );
}
