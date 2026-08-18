import { capitalize } from "@/shared/lib";

import type { ParsedCatalogSlug } from "../model/types";

const VALID_GENDERS = ["men", "women", "kids", "unisex"];

const VALID_CATEGORIES = ["sneakers", "running", "boots", "sandals", "shoes"];

export function parseCatalogSlug(slug: string): ParsedCatalogSlug {
  const parts = slug.toLowerCase().split("-").filter(Boolean);

  if (parts.length === 1) {
    const [part] = parts;

    if (VALID_GENDERS.includes(part)) {
      return {
        gender: capitalize(part),
        isValid: true,
      };
    }

    if (VALID_CATEGORIES.includes(part)) {
      return {
        category: part,
        isValid: true,
      };
    }
  }

  if (parts.length === 2) {
    const [gender, category] = parts;

    if (VALID_GENDERS.includes(gender) && VALID_CATEGORIES.includes(category)) {
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
