"use client";

import { ProductCard } from "@/entities/product";
import { useGetProductsQuery } from "@/entities/product";
import { useFilters } from "@/features/catalog-filters";

export function ProductGrid() {
  const filters = useFilters();
  const { data, isLoading, isError } = useGetProductsQuery(filters);

  if (isLoading) {
    return (
      <div className="grid grid-cols-1 items-start gap-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4">
        {Array.from({ length: 8 }).map((_, i) => (
          <div
            key={i}
            className="aspect-square w-full animate-pulse bg-muted"
          />
        ))}
      </div>
    );
  }

  if (isError) {
    return (
      <div className="flex min-h-60 items-center justify-center text-muted-foreground">
        Something went wrong. Please try again.
      </div>
    );
  }

  if (!data || data.items.length === 0) {
    return (
      <div className="flex min-h-60 items-center justify-center text-muted-foreground">
        No products found.
      </div>
    );
  }

  return (
    <div className="grid grid-cols-1 items-start gap-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4">
      {data.items.map((product) => (
        <ProductCard key={product.productId} product={product} />
      ))}
    </div>
  );
}
