"use client";

import {
  Pagination,
  PaginationContent,
  PaginationItem,
  PaginationPrevious,
  PaginationNext,
  PaginationLink,
} from "@/shared/ui";
import { useGetProductsQuery } from "@/entities/product";
import { useFilters } from "@/features/catalog-filters";
import { useUpdateQuery } from "@/features/catalog-filters/lib/use-update-query";

export function CatalogPagination() {
  const filters = useFilters();
  const { data } = useGetProductsQuery(filters);
  const updateQuery = useUpdateQuery();

  if (!data || data.totalPages <= 1) return null;

  const { pageNumber, totalPages, hasPreviousPage, hasNextPage } = data;

  const goTo = (page: number) =>
    updateQuery({ page: page === 1 ? null : page });

  // окно из максимум 5 номеров вокруг текущей
  const pages = Array.from({ length: totalPages }, (_, i) => i + 1).filter(
    (p) => p === 1 || p === totalPages || Math.abs(p - pageNumber) <= 1,
  );

  return (
    <Pagination className="mt-12">
      <PaginationContent>
        <PaginationItem>
          <PaginationPrevious
            onClick={() => hasPreviousPage && goTo(pageNumber - 1)}
            className={
              !hasPreviousPage
                ? "pointer-events-none opacity-50"
                : "cursor-pointer"
            }
          />
        </PaginationItem>

        {pages.map((p, i) => {
          const prev = pages[i - 1];
          const gap = prev && p - prev > 1;
          return (
            <PaginationItem key={p}>
              {gap && <span className="px-2 text-muted-foreground">…</span>}
              <PaginationLink
                isActive={p === pageNumber}
                onClick={() => goTo(p)}
                className="cursor-pointer"
              >
                {p}
              </PaginationLink>
            </PaginationItem>
          );
        })}

        <PaginationItem>
          <PaginationNext
            onClick={() => hasNextPage && goTo(pageNumber + 1)}
            className={
              !hasNextPage ? "pointer-events-none opacity-50" : "cursor-pointer"
            }
          />
        </PaginationItem>
      </PaginationContent>
    </Pagination>
  );
}
