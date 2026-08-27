import { BrandPageClient } from "@/widgets/brand-page";

interface PageProps {
  params: Promise<{
    brandSlug: string;
  }>;
}

export default async function BrandPage({ params }: PageProps) {
  const { brandSlug } = await params;

  return <BrandPageClient brandSlug={brandSlug} />;
}
