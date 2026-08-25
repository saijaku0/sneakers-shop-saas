"use client";

import { useState } from "react";
import Link from "next/link";
import { useSearchParams } from "next/navigation";
import { Star } from "lucide-react";
import type {
  ProductDetail as ProductDetailType,
  ProductSize,
} from "@/entities/product";
import { ProductGallery } from "./product-gallery";
import { ColorSwatches } from "./color-swatches";
import { SizePicker } from "./size-picker";
import { AddToCartButton } from "./add-to-cart-button";
import { ProductInfoAccordion } from "./product-info-accordion";

const GENDER = ["Men", "Women", "Unisex", "Kids"];

export function ProductDetail({ product }: { product: ProductDetailType }) {
  const searchParams = useSearchParams();
  const variantFromUrl = searchParams.get("variant");

  const initialVariant =
    product.variants.find((v) => v.variantId === variantFromUrl)?.variantId ??
    product.variants[0]?.variantId;

  const [variantId, setVariantId] = useState(initialVariant);
  const [selectedSize, setSelectedSize] = useState<ProductSize | null>(null);

  const variant =
    product.variants.find((v) => v.variantId === variantId) ??
    product.variants[0];

  const genderLabel = GENDER[product.gender] ?? "";
  const genderCatSlug = `/${genderLabel.toLowerCase()}`;
  const catSlug = `/${genderLabel.toLowerCase()}-${product.categoryName.toLowerCase()}`;

  const handleColorChange = (id: string) => {
    setVariantId(id);
    setSelectedSize(null);
  };

  return (
    <div className="mx-auto px-4 py-6">
      <nav className="mb-6 flex items-center gap-2 text-sm text-muted-foreground">
        <Link href="/" className="hover:text-foreground hover:underline">
          Home
        </Link>
        <span>/</span>
        <Link
          href={genderCatSlug}
          className="hover:text-foreground hover:underline"
        >
          <span>{genderLabel}</span>
        </Link>
        <span>/</span>
        <Link href={catSlug} className="hover:text-foreground hover:underline">
          {product.categoryName}
        </Link>
      </nav>

      <div className="grid gap-8 lg:grid-cols-[1fr_400px]">
        <ProductGallery
          key={variant.variantId}
          images={variant.images}
          alt={product.model}
        />

        <div className="lg:sticky lg:top-24 lg:h-fit">
          <p className="text-sm text-muted-foreground">{product.brandName}</p>

          <div className="mt-2 flex items-center gap-1.5 text-sm">
            <span className="font-medium text-foreground">4.8</span>
            <div className="flex">
              {Array.from({ length: 5 }).map((_, i) => (
                <Star
                  key={i}
                  className="h-3.5 w-3.5 fill-foreground text-foreground"
                />
              ))}
            </div>
            <span className="text-muted-foreground underline">(5004)</span>
          </div>

          <h1 className="mt-3 font-display text-3xl font-bold uppercase tracking-tight text-foreground">
            {product.model}
          </h1>

          <p className="mt-3 text-xl font-bold text-foreground">
            € {product.basePrice}
          </p>

          <div className="mt-6">
            <ColorSwatches
              variants={product.variants}
              selectedId={variant.variantId}
              onSelect={handleColorChange}
            />
          </div>

          <div className="mt-6">
            <SizePicker
              sizes={variant.sizes}
              selected={selectedSize}
              onSelect={setSelectedSize}
            />
          </div>

          <p className="mt-3 text-xs text-muted-foreground">
            <span className="font-medium text-foreground">True to size.</span>{" "}
            We recommend your usual size.
          </p>

          <div className="mt-6">
            <AddToCartButton
              product={product}
              variant={variant}
              selectedSize={selectedSize}
            />
          </div>

          {product.description && (
            <div className="mt-8 border-t pt-6">
              <h2 className="mb-2 text-sm font-medium uppercase tracking-wide text-foreground">
                Description
              </h2>
              <p className="text-sm leading-relaxed text-muted-foreground">
                {product.description}
              </p>
            </div>
          )}
        </div>
        <ProductInfoAccordion image={variant.images[0]} />
      </div>
    </div>
  );
}
