import { CatalogFilters } from "../model/types";

export function buildApiParams(filters: CatalogFilters): URLSearchParams {
  const params = new URLSearchParams();

  filters.colors.forEach((c) => params.append("colors", c));
  filters.sizes.forEach((s) => params.append("sizes", String(s)));
  filters.brands.forEach((b) => params.append("brands", b));
  filters.categories.forEach((c) => params.append("categories", c));

  if (filters.gender) params.append("gender", filters.gender);
  if (filters.minPrice !== undefined)
    params.append("minPrice", String(filters.minPrice));
  if (filters.maxPrice !== undefined)
    params.append("maxPrice", String(filters.maxPrice));
  if (filters.sortBy && filters.sortBy !== "popular")
    params.append("sortBy", filters.sortBy);
  params.append("pageNumber", String(filters.page));

  return params;
}
