"use client";

import { useGetProductsQuery } from "@/entities/product";
import { useFilters } from "@/features/catalog-filters";

export function CatalogHeader() {
  const filters = useFilters();
  const { data } = useGetProductsQuery(filters);

  const category = filters.categories[0] ?? "";
  const title = [filters.gender, category].filter(Boolean).join(" ");

  return (
    <div className="flex items-baseline gap-3">
      <h1 className="font-display text-2xl font-bold uppercase tracking-tight text-foreground">
        {title || "All Products"}
      </h1>
      {data && (
        <span className="text-sm text-muted-foreground">
          {data.totalCount} {data.totalCount === 1 ? "item" : "items"}
        </span>
      )}
    </div>
  );
}
