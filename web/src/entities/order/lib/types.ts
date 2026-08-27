import type { Address } from "@/entities/profile";

export enum PaymentMethod {
  CreditCard = 0,
  DebitCard = 1,
  PayPal = 2,
  Cash = 3,
}

export type PaymentMethodName = "CreditCard" | "DebitCard" | "PayPal" | "Cash";

export interface CreateOrderRequest {
  shippingAddress: Address;
  paymentMethod: PaymentMethod;
}

export interface OrderItem {
  warehouseItemId: string;
  model: string;
  brandName: string;
  color: string;
  sizeCm: number;
  previewImageUrl: string;
  unitPrice: number;
  quantity: number;
  discountAmount: number;
  totalPrice: number;
}

export interface OrderDetail {
  id: string;
  status: string;
  totalAmount: number;
  paymentMethod: PaymentMethodName;
  paymentDeadline: string;
  createdAt: string;
  shippingAddress: Address;
  items: OrderItem[];
}

export interface OrderSummary {
  id: string;
  status: string;
  totalAmount: number;
  createdAt: string;
  itemCount: number;
  previewImages: string[];
  itemsPreviewText: string;
}
