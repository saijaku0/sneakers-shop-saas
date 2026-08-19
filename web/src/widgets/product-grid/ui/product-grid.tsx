import { ProductCard } from "@/entities/product";
import { mockProductsDb } from "../model/mock-product-list";

export function ProductGrid() {
  return (
    <div className="grid grid-cols-1 items-start gap-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4">
      {mockProductsDb.map((product) => (
        <ProductCard key={product.id} product={product} />
      ))}
    </div>
  );
}
