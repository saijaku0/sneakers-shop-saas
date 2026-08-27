import { OrderHistory } from "@/widgets/order-history";

export default function OrdersPage() {
  return (
    <div className="flex flex-col gap-6 py-8">
      <h1 className="font-display text-2xl font-bold uppercase tracking-tight text-foreground">
        Order History
      </h1>
      <OrderHistory />
    </div>
  );
}
