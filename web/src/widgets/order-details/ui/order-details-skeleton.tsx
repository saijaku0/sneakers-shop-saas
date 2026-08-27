import { Skeleton } from "@/shared/ui";

export function OrderDetailsSkeleton() {
  return (
    <div className="flex flex-col gap-6 py-8">
      <Skeleton className="h-4 w-28" />

      <div className="flex items-start justify-between gap-4">
        <div className="flex flex-col gap-2">
          <Skeleton className="h-7 w-48" />
          <Skeleton className="h-4 w-40" />
        </div>
        <Skeleton className="h-7 w-20" />
      </div>

      <div className="rounded-md border bg-card p-4">
        <Skeleton className="h-4 w-24" />
        <div className="mt-3 flex flex-col gap-4">
          {Array.from({ length: 2 }).map((_, index) => (
            <div key={index} className="flex items-start gap-4">
              <Skeleton className="h-16 w-16 shrink-0 rounded-md" />
              <div className="flex flex-1 flex-col gap-2">
                <Skeleton className="h-3 w-20" />
                <Skeleton className="h-4 w-40" />
                <Skeleton className="h-3 w-28" />
              </div>
            </div>
          ))}
        </div>
      </div>

      <div className="grid gap-4 sm:grid-cols-2">
        <Skeleton className="h-28 w-full rounded-md" />
        <Skeleton className="h-28 w-full rounded-md" />
      </div>
      <Skeleton className="h-16 w-full rounded-md" />
    </div>
  );
}
