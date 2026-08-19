export interface ProductListItem {
  id: string;
  model: string;
  brand: string;
  basePrice: number;
  variantsCount: number;
  variants: ProductVariantPreview[];
}

export interface ProductVariantPreview {
  variantId: string;
  colorName: string;
  imageUrl: string;
}
