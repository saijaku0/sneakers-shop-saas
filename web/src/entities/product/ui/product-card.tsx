/* eslint-disable @next/next/no-img-element */
"use client";

import { ProductListItem } from "../model/types";

interface ProductCardProps {
  product: ProductListItem;
  children?: React.ReactNode;
}

export function ProductCard({ product, children }: ProductCardProps) {
  return (
    <article className="group flex flex-col cursor-pointer border border-transparent hover:border-foreground transition-all duration-100 hover:scale-y-[1.05] origin-top hover:z-50">
      <div className="relative aspect-4/3 sm:aspect-square w-full bg-background overflow-hidden">
        <img
          src={product.imageUrl}
          alt={product.model}
          className="h-full w-full object-cover object-center transition-transform duration-500"
        />
      </div>

      <div className="flex flex-col pt-3 pb-1 text-left p-2 bg-background">
        <span className="text-sm font-bold text-foreground mb-1">
          € {product.basePrice}
        </span>

        <h3 className="text-sm font-normal text-foreground leading-tight">
          {product.model}
        </h3>

        <span className="text-sm text-foreground mt-0.5">{product.brand}</span>

        <span className="text-sm text-foreground mt-0.5">
          {product.variantsCount}{" "}
          {product.variantsCount === 1 ? "colour" : "colours"}
        </span>

        <div className="mt-4">{children}</div>
      </div>
    </article>
  );
}
