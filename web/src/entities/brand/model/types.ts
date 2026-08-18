export interface Brand {
  name: string;
  slug: string;
}

export interface ParsedBrandSlug {
  brand?: Brand;
  isValid: boolean;
}
