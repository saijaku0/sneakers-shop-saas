interface CartItem {
  warehouseItemId: string;
  productId: string;
  model: string;
  brandName: string;
  sizeCm: number;
  color: string;
  unitPrice: number;
  quantity: number;
  previewImageUrl: string;
  isAvailable: boolean;
  available?: number;
}

interface Cart {
  cartId: string;
  userId: string;
  items: CartItem[];
  subtotal: number;
  itemCount: number;
}

interface LocalCartItem {
  warehouseItemId: string;
  productId: string;
  model: string;
  brandName: string;
  sizeCm: number;
  color: string;
  unitPrice: number;
  quantity: number;
  previewImageUrl: string;
  isAvailable: boolean;
}

interface AddToCartRequest {
  warehouseItemId: string;
  quantity: number;
}

interface UpdateCartQuantityRequest {
  warehouseItemId: string;
  quantity: number;
}

export type {
  Cart,
  CartItem,
  AddToCartRequest,
  UpdateCartQuantityRequest,
  LocalCartItem,
};
