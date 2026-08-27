import { CategoryDto } from "@/entities/category";
import { BrandDto } from "@/entities/brand";

export interface NavData {
  categories: {
    men: string[];
    women: string[];
    kids: string[];
  };
  brands: Array<{ name: string; slug: string }>;
}

export const createSlug = (name: string): string => {
  return name.toLowerCase().replace(/\s+/g, "-");
};

export const adaptNavData = (
  rawCategories?: CategoryDto[],
  rawBrands?: BrandDto[],
): NavData => {
  const categoryNames = rawCategories ? rawCategories.map((c) => c.name) : [];

  const brands = rawBrands
    ? rawBrands.map((b) => ({
        name: b.name,
        slug: createSlug(b.name),
      }))
    : [];

  return {
    categories: {
      men: categoryNames,
      women: categoryNames,
      kids: categoryNames.filter((name) =>
        ["Sneakers", "Running"].includes(name),
      ),
    },
    brands,
  };
};
