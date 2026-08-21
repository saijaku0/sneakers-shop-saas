"use client";

import {
  Checkbox,
  Label,
  Button,
  Accordion,
  AccordionItem,
  AccordionTrigger,
  AccordionContent,
} from "@/shared/ui";
import { useGetFiltersQuery } from "@/entities/product";
import { useFilters } from "../lib/use-filters";
import { useUpdateQuery } from "../lib/use-update-query";
import { PriceSlider } from "./price-slider";

export function FilterPanel() {
  const { data: facets, isLoading } = useGetFiltersQuery();
  const filters = useFilters();
  const updateQuery = useUpdateQuery();

  if (isLoading || !facets) {
    return <div className="h-96 w-full animate-pulse rounded-md bg-muted" />;
  }

  const toggle = (key: string, value: string, current: string[]) => {
    const safeCurrent = current || [];
    const next = safeCurrent.includes(value)
      ? safeCurrent.filter((v) => v !== value)
      : [...safeCurrent, value];

    updateQuery({ [key]: next.length ? next.join(",") : null, page: null });
  };

  const safeBrands = filters.brands || [];
  const safeColors = filters.colors || [];
  const safeSizes = filters.sizes || [];

  const hasActiveFilters =
    safeColors.length > 0 ||
    safeBrands.length > 0 ||
    safeSizes.length > 0 ||
    filters.minPrice !== undefined ||
    filters.maxPrice !== undefined;

  const clearAll = () =>
    updateQuery({
      colors: null,
      brands: null,
      sizes: null,
      minPrice: null,
      maxPrice: null,
      page: null,
    });

  return (
    <aside className="sticky top-24 flex h-[calc(100vh-8rem)] w-full flex-col gap-4 overflow-y-auto pr-2 pb-8 scrollbar-thin scrollbar-thumb-muted-foreground/20 scrollbar-track-transparent hover:scrollbar-thumb-muted-foreground/40">
      <div className="flex items-center">
        <Button
          variant="link"
          size="sm"
          onClick={clearAll}
          className={`self-start p-0 text-muted-foreground transition-all duration-300 hover:text-foreground ${
            hasActiveFilters
              ? "opacity-100 translate-y-0 visible"
              : "opacity-0 -translate-y-1 invisible"
          }`}
        >
          Clear all
        </Button>
      </div>

      <Accordion type="multiple" className="w-full">
        <FilterGroup value="brand" title="Brand">
          {facets.brands.map((brand) => (
            <div key={brand} className="flex items-center gap-2">
              <Checkbox
                id={`brand-${brand}`}
                checked={safeBrands.includes(brand)}
                onCheckedChange={() => toggle("brands", brand, safeBrands)}
              />
              <Label
                htmlFor={`brand-${brand}`}
                className="cursor-pointer text-sm font-normal text-foreground"
              >
                {brand}
              </Label>
            </div>
          ))}
        </FilterGroup>

        <FilterGroup value="color" title="Color">
          {facets.colors.map((color) => (
            <div key={color} className="flex items-center gap-2">
              <Checkbox
                id={`color-${color}`}
                checked={safeColors.includes(color)}
                onCheckedChange={() => toggle("colors", color, safeColors)}
              />
              <Label
                htmlFor={`color-${color}`}
                className="cursor-pointer text-sm font-normal text-foreground"
              >
                {color}
              </Label>
            </div>
          ))}
        </FilterGroup>

        <FilterGroup value="size" title="Size (cm)">
          <div className="grid grid-cols-3 gap-2">
            {facets.sizes.map((size) => {
              const sizeStr = String(size);
              const active = safeSizes.map(String).includes(sizeStr);

              return (
                <button
                  key={sizeStr}
                  type="button"
                  onClick={() =>
                    toggle("sizes", sizeStr, safeSizes.map(String))
                  }
                  className={`h-10 border text-sm transition-colors ${
                    active
                      ? "border-foreground bg-foreground text-background"
                      : "border-border hover:border-foreground"
                  }`}
                >
                  {sizeStr}
                </button>
              );
            })}
          </div>
        </FilterGroup>

        <FilterGroup value="price" title="Price (€)">
          <PriceSlider
            min={facets.priceRange.min}
            max={facets.priceRange.max}
            value={[
              filters.minPrice ?? facets.priceRange.min,
              filters.maxPrice ?? facets.priceRange.max,
            ]}
            onCommit={([min, max]) =>
              updateQuery({
                minPrice: min === facets.priceRange.min ? null : min,
                maxPrice: max === facets.priceRange.max ? null : max,
                page: null,
              })
            }
          />
        </FilterGroup>
      </Accordion>
    </aside>
  );
}

function FilterGroup({
  value,
  title,
  children,
}: {
  value: string;
  title: string;
  children: React.ReactNode;
}) {
  return (
    <AccordionItem value={value} className="border-none">
      <AccordionTrigger className="py-4 hover:no-underline">
        <span className="text-sm font-medium uppercase tracking-wide text-foreground">
          {title}
        </span>
      </AccordionTrigger>
      <AccordionContent className="pb-4">
        <div className="flex flex-col gap-2.5">{children}</div>
      </AccordionContent>
    </AccordionItem>
  );
}
