/* eslint-disable @next/next/no-img-element */
"use client";

import { useState } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { ProductListItem } from "../model/types";

interface ProductCardProps {
  product: ProductListItem;
}

export function ProductCard({ product }: ProductCardProps) {
  const router = useRouter();
  const [activeImage, setActiveImage] = useState(product.variants[0].imageUrl);

  const href = `/products/${product.productId}`;

  return (
    <article
      className="group relative flex flex-col cursor-pointer"
      onMouseLeave={() => setActiveImage(product.variants[0].imageUrl)}
    >
      <div
        className="absolute inset-x-0 top-0 z-0 flex flex-col border border-transparent
                      bg-background group-hover:z-20 group-hover:border-foreground transition-colors"
      >
        <Link
          href={href}
          className="relative aspect-4/3 sm:aspect-square w-full bg-background overflow-hidden"
          aria-label={product.model}
        >
          <img
            src={activeImage}
            alt={product.model}
            className="h-full w-full object-cover object-center"
          />
        </Link>

        <div className="hidden group-hover:flex py-1">
          {product.variants.map((v) => (
            <button
              key={v.variantId}
              type="button"
              onMouseEnter={() => setActiveImage(v.imageUrl)}
              onFocus={() => setActiveImage(v.imageUrl)}
              onClick={() => router.push(`${href}?variant=${v.variantId}`)}
              aria-label={v.colorName}
              className="h-9 w-9 shrink-0 overflow-hidden bg-background border-b-2 hover:border-b-foreground transition-colors cursor-pointer"
            >
              <img
                src={v.imageUrl}
                alt={v.colorName}
                className="h-full w-full object-cover"
              />
            </button>
          ))}
        </div>

        <Link href={href} className="flex flex-col p-2 pt-3 pb-1 text-left">
          <div className="flex justify-between items-center">
            <span className="text-sm font-bold text-foreground mb-1">
              € {product.basePrice}
            </span>
            <span className="text-sm text-foreground/60 mt-0.5">
              {product.variantsCount}{" "}
              {product.variantsCount === 1 ? "colour" : "colours"}
            </span>
          </div>
          <h3 className="font-normal text-foreground leading-tight">
            {product.brandName} {product.model}
          </h3>
        </Link>
      </div>

      <div className="invisible flex flex-col" aria-hidden>
        <div className="aspect-4/3 sm:aspect-square w-full" />
        <div className="flex flex-col p-2 pt-3 pb-1">
          <div className="flex justify-between items-center">
            <span className="text-sm font-bold mb-1">{product.basePrice}</span>
            <span className="text-sm mt-0.5">.</span>
          </div>
          <h3 className="font-normal leading-tight">{product.model}</h3>
          <span className="text-sm mt-0.5">{product.brandName}</span>
        </div>
      </div>
    </article>
  );
}
