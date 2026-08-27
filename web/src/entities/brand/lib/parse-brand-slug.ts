import type { Brand, ParsedBrandSlug } from "../model/types";

export function parseBrandSlug(slug: string, brands: Brand[]): ParsedBrandSlug {
  const normalizedSlug = slug.toLowerCase();

  const brand = brands.find(
    (item) => item.slug.toLowerCase() === normalizedSlug,
  );

  if (!brand) {
    return {
      isValid: false,
    };
  }

  return {
    brand,
    isValid: true,
  };
}
