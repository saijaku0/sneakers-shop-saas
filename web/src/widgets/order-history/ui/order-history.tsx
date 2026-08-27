"use client";

import { useGetOrdersQuery } from "@/entities/order";
import { OrderCard } from "./order-card";
import { OrderHistorySkeleton } from "./order-history-skeleton";
import { OrderHistoryEmpty } from "./order-history-empty";

export function OrderHistory() {
  const { data, isLoading, isError } = useGetOrdersQuery();

  if (isLoading) {
    return <OrderHistorySkeleton />;
  }

  if (isError) {
    return (
      <div className="flex min-h-60 items-center justify-center text-muted-foreground">
        Something went wrong. Please try again.
      </div>
    );
  }

  if (!data || data.length === 0) {
    return <OrderHistoryEmpty />;
  }

  return (
    <div className="flex flex-col gap-4">
      {data.map((order) => (
        <OrderCard key={order.id} order={order} />
      ))}
    </div>
  );
}
