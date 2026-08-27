"use client";

import { useMemo } from "react";
import { notFound } from "next/navigation";
import { useGetCategoriesQuery } from "@/entities/category";
import { parseCatalogSlug } from "@/entities/catalog";
import { CatalogBreadcrumbs, Container } from "@/shared/ui";
import { ProductGrid } from "@/widgets/product-grid";
import { CatalogPagination } from "@/features/catalog-pagination";
import { FilterPanel, SortSelect } from "@/features/catalog-filters";
import { CatalogHeader } from "@/features/catalog";

interface CatalogRouteProps {
  catalogSlug: string;
}

export function CatalogRoute({ catalogSlug }: CatalogRouteProps) {
  const { data: categories, isLoading } = useGetCategoriesQuery();

  const parsed = useMemo(
    () => parseCatalogSlug(catalogSlug, categories ?? []),
    [catalogSlug, categories],
  );

  if (isLoading) {
    return null;
  }

  if (!parsed.isValid) {
    notFound();
  }

  const { gender, category } = parsed;

  return (
    <Container>
      <div className="container mx-auto py-8">
        <CatalogBreadcrumbs gender={gender} category={category} />

        <div className="flex flex-col gap-8 md:flex-row">
          <div className="mx-auto flex max-w-7xl gap-8 px-4 py-6">
            <div className="hidden w-64 shrink-0 lg:block">
              <FilterPanel />
            </div>
          </div>

          <main className="flex-1">
            <header className="sticky top-16 z-30 mb-6 flex items-center justify-between gap-4 bg-background py-4">
              <CatalogHeader />
              <SortSelect />
            </header>

            <ProductGrid />
            <CatalogPagination />
          </main>
        </div>
      </div>
    </Container>
  );
}
