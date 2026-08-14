interface CartItemImage {
  url: string;
  alt: string;
}

interface CartItem {
  warehouseItemId: string;
  productId: string;
  name: string;
  brand: string;
  size: number;
  color: string;
  unitPrice: number;
  quantity: number;
  image: CartItemImage;
  inStock: boolean;
  maxAvailable: number;
}

interface Cart {
  cartId: string;
  userId: string;
  items: CartItem[];
  subtotal: number;
  itemCount: number;
}

export { type Cart, type CartItem };
