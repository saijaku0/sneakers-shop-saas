"use client";

import { useMemo } from "react";
import { useParams, useSearchParams } from "next/navigation";
import { parseCatalogSlug } from "./parse-catalog-slug";
import { CatalogFilters } from "@/entities/product";

const DELIMITER = ",";

export function useFilters(): CatalogFilters {
  const searchParams = useSearchParams();
  const params = useParams();

  return useMemo(() => {
    const slug = String(params.catalogSlug ?? "");
    const { gender, category } = parseCatalogSlug(slug);

    const list = (key: string) =>
      searchParams.get(key)?.split(DELIMITER).filter(Boolean) ?? [];
    const num = (key: string) => {
      const raw = searchParams.get(key);
      return raw ? Number(raw) : undefined;
    };

    return {
      gender,
      categories: category ? [category] : [],
      colors: list("colors"),
      sizes: list("sizes").map(Number),
      brands: list("brands"),
      minPrice: num("minPrice"),
      maxPrice: num("maxPrice"),
      sortBy: searchParams.get("sortBy") ?? "newest",
      page: num("page") ?? 1,
    };
  }, [params.catalogSlug, searchParams]);
}
