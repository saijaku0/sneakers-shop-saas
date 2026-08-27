import { Skeleton } from "@/shared/ui";

export function OrderConfirmationSkeleton() {
  return (
    <div className="mx-auto w-full max-w-160 px-4 py-10 sm:px-6">
      <div className="mb-8 flex flex-col gap-3">
        <Skeleton className="h-4 w-32" />
        <Skeleton className="h-8 w-64" />
      </div>

      <div className="flex flex-col gap-4">
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

        <Skeleton className="h-32 w-full rounded-md" />
        <Skeleton className="h-24 w-full rounded-md" />
      </div>
    </div>
  );
}
