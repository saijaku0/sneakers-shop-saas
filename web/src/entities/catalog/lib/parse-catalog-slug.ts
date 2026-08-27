import { capitalize } from "@/shared/lib";
import type { ParsedCatalogSlug } from "../model/types";

const VALID_GENDERS = ["men", "women", "kids", "unisex"];

export function parseCatalogSlug(
  slug: string,
  categories: Array<{ name: string }>,
): ParsedCatalogSlug {
  const parts = slug.toLowerCase().split("-").filter(Boolean);

  const validCategories = categories.map((category) =>
    category.name.toLowerCase(),
  );

  if (parts.length === 1) {
    const [part] = parts;

    if (VALID_GENDERS.includes(part)) {
      return {
        gender: capitalize(part),
        isValid: true,
      };
    }

    if (validCategories.includes(part)) {
      return {
        category: part,
        isValid: true,
      };
    }
  }

  if (parts.length === 2) {
    const [gender, category] = parts;

    if (VALID_GENDERS.includes(gender) && validCategories.includes(category)) {
      return {
        gender: capitalize(gender),
        category,
        isValid: true,
      };
    }
  }

  return {
    isValid: false,
  };
}
