import type { Brand, ParsedBrandSlug } from "../model/types";

export function parseBrandSlug(slug: string): ParsedBrandSlug {
  const brands: Brand[] = [
    { name: "Nike", slug: "nike" },
    { name: "Adidas", slug: "adidas" },
    { name: "New Balance", slug: "new-balance" },
    { name: "Puma", slug: "puma" },
    { name: "Reebok", slug: "reebok" },
  ];

  const brand = brands.find((item) => item.slug === slug.toLowerCase());

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
