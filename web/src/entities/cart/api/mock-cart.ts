import { Cart } from "../model/types";

export const mockCart: Cart = {
  cartId: "0192f8c1-4a2b-7d3e-9f10-2c3d4e5f6a7b",
  userId: "0192f8c1-1111-7d3e-9f10-2c3d4e5f6a7b",
  items: [
    {
      warehouseItemId: "81088402-19d5-4b0b-825d-2dadfba34a6b",
      productId: "a1b2c3d4-0001-4000-8000-000000000001",
      name: "Air Zoom Pegasus 41",
      brand: "Nike",
      size: 43,
      color: "Black / Volt",
      unitPrice: 139.99,
      quantity: 1,
      image: {
        url: "https://example.com/pegasus41.jpg",
        alt: "Nike Air Zoom Pegasus 41",
      },
      inStock: true,
      maxAvailable: 4,
    },
    {
      warehouseItemId: "81088402-19d5-4b0b-825d-2dadfba34a6c",
      productId: "a1b2c3d4-0002-4000-8000-000000000002",
      name: "Ultraboost Light",
      brand: "Adidas",
      size: 42,
      color: "Cloud White",
      unitPrice: 189.99,
      quantity: 2,
      image: {
        url: "https://example.com/ultraboost.jpg",
        alt: "Adidas Ultraboost Light",
      },
      inStock: true,
      maxAvailable: 7,
    },
    {
      warehouseItemId: "81088402-19d5-4b0b-825d-2dadfba34a6d",
      productId: "a1b2c3d4-0003-4000-8000-000000000003",
      name: "574 Core",
      brand: "New Balance",
      size: 44,
      color: "Grey",
      unitPrice: 99.99,
      quantity: 1,
      image: {
        url: "https://example.com/nb574.jpg",
        alt: "New Balance 574 Core",
      },
      inStock: false,
      maxAvailable: 0,
    },
  ],
  subtotal: 619.96,
  itemCount: 4,
};
