"use client";

import { use } from "react";
import { notFound } from "next/navigation";
import { useGetOrderByIdQuery } from "@/entities/order";
import {
  OrderConfirmation,
  OrderConfirmationSkeleton,
} from "@/widgets/order-confirmation";

export default function OrderPage({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = use(params);
  const { data, isLoading, isError, error } = useGetOrderByIdQuery(id);

  if (isLoading) {
    return <OrderConfirmationSkeleton />;
  }

  if (isError && "status" in error && error.status === 404) {
    notFound();
  }

  if (isError || !data) {
    return (
      <div className="flex min-h-60 items-center justify-center text-muted-foreground">
        Something went wrong. Please try again.
      </div>
    );
  }

  return <OrderConfirmation order={data} />;
}
