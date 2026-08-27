export interface ProductListItem {
  productId: string;
  model: string;
  brandName: string;
  basePrice: number;
  variantsCount: number;
  variants: ProductVariantPreview[];
}

export interface ProductVariantPreview {
  variantId: string;
  colorName: string;
  imageUrl: string;
}

export interface CatalogFilters {
  colors: string[];
  sizes: number[];
  brands: string[];
  categories: string[];
  gender: string;
  minPrice?: number;
  maxPrice?: number;
  sortBy: string;
  page: number;
}

export interface ProductsPageResult {
  items: ProductListItem[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

export interface ProductSize {
  warehouseItemId: string;
  sizeCm: number;
  inStock: boolean;
}

export interface ProductDetailVariant {
  variantId: string;
  colorName: string;
  images: string[];
  sizes: ProductSize[];
}

export interface ProductDetail {
  id: string;
  model: string;
  brandName: string;
  categoryName: string;
  gender: number;
  basePrice: number;
  description: string;
  variants: ProductDetailVariant[];
}

export interface ProductFilters {
  brands: string[];
  categories: string[];
  colors: string[];
  sizes: number[];
  priceRange: { min: number; max: number };
}
