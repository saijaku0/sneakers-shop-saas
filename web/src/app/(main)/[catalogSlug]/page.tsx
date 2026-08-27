import { CatalogRoute } from "@/widgets/catalog-route";

interface PageProps {
  params: Promise<{
    catalogSlug: string;
  }>;
}

export default async function DynamicCatalogRoute({ params }: PageProps) {
  const { catalogSlug } = await params;

  return <CatalogRoute catalogSlug={catalogSlug} />;
}
