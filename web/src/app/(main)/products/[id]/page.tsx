"use client";

import { use } from "react";
import { notFound } from "next/navigation";
import { useGetProductByIdQuery } from "@/entities/product";
import { ProductDetail } from "@/features/product-detail";

export default function ProductPage({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = use(params);
  const { data, isLoading, isError, error } = useGetProductByIdQuery(id);

  if (isLoading) {
    return (
      <div className="mx-auto grid max-w-6xl gap-8 px-4 py-10 lg:grid-cols-2">
        <div className="aspect-square w-full animate-pulse bg-muted" />
        <div className="flex flex-col gap-4">
          <div className="h-8 w-2/3 animate-pulse bg-muted" />
          <div className="h-6 w-1/3 animate-pulse bg-muted" />
          <div className="h-24 w-full animate-pulse bg-muted" />
        </div>
      </div>
    );
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

  return <ProductDetail product={data} />;
}
