import { notFound } from "next/navigation";

import { parseBrandSlug } from "@/entities/brand";

interface PageProps {
  params: Promise<{
    brandSlug: string;
  }>;
}

export default async function BrandPage({ params }: PageProps) {
  const { brandSlug } = await params;

  const { brand, isValid } = parseBrandSlug(brandSlug);

  if (!isValid || !brand) {
    notFound();
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold">{brand.name}</h1>

      <div className="flex flex-col gap-8 md:flex-row">
        <aside className="w-full shrink-0 md:w-64">Brand filters</aside>

        <main className="flex-1">Product grid for {brand.name}</main>
      </div>
    </div>
  );
}
