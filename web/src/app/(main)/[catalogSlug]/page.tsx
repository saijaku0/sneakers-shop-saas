import { notFound } from "next/navigation";

import { parseCatalogSlug } from "@/entities/catalog";
import { CatalogBreadcrumbs, Container } from "@/shared/ui";
import { ProductGrid } from "@/widgets/product-grid";

interface PageProps {
  params: Promise<{
    catalogSlug: string;
  }>;
}

export default async function DynamicCatalogRoute({ params }: PageProps) {
  const { catalogSlug } = await params;

  const { gender, category, isValid } = parseCatalogSlug(catalogSlug);
  if (!isValid) {
    notFound();
  }

  return (
    <Container className="pt-12">
      <div className="container mx-auto py-8">
        <CatalogBreadcrumbs gender={gender} category={category} />

        <div className="flex flex-col gap-8 md:flex-row">
          <aside className="w-full shrink-0 md:w-64">Catalog widget</aside>

          <main className="flex-1">
            <ProductGrid />
          </main>
        </div>
      </div>
    </Container>
  );
}
