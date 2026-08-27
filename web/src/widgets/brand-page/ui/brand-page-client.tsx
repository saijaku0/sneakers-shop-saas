"use client";

import { useMemo } from "react";
import { useGetBrandsQuery } from "@/entities/brand";
import { parseBrandSlug } from "@/entities/brand";
import { Container } from "@/shared/ui";

interface BrandPageClientProps {
  brandSlug: string;
}

const createSlug = (name: string) => name.toLowerCase().replace(/\s+/g, "-");

export function BrandPageClient({ brandSlug }: BrandPageClientProps) {
  const { data: brands, isLoading } = useGetBrandsQuery();

  const parsed = useMemo(() => {
    const normalizedBrands =
      brands?.map((brand) => ({
        ...brand,
        slug: createSlug(brand.name),
      })) ?? [];

    return parseBrandSlug(brandSlug, normalizedBrands);
  }, [brandSlug, brands]);

  if (isLoading) {
    return null;
  }

  if (!parsed.isValid || !parsed.brand) {
    return <div>404</div>;
  }

  const { brand } = parsed;

  return (
    <Container className="pt-12">
      <div className="container mx-auto px-4 py-8">
        <h1 className="text-3xl font-bold">{brand.name}</h1>

        <div className="flex flex-col gap-8 md:flex-row">
          <aside className="w-full shrink-0 md:w-64">Brand filters</aside>

          <main className="flex-1">Product grid for {brand.name}</main>
        </div>
      </div>
    </Container>
  );
}
