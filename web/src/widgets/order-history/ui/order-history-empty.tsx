import Link from "next/link";
import { PackageSearch } from "lucide-react";
import { Button } from "@/shared/ui";

export function OrderHistoryEmpty() {
  return (
    <div className="flex flex-col items-center justify-center gap-4 rounded-md border border-dashed py-20 text-center">
      <PackageSearch className="size-10 text-muted-foreground" />
      <div className="flex flex-col gap-1">
        <p className="font-display text-lg font-bold uppercase tracking-tight text-foreground">
          No orders yet
        </p>
        <p className="text-sm text-muted-foreground">
          Your future purchases will show up here.
        </p>
      </div>
      <Button asChild>
        <Link href="/">Browse catalog</Link>
      </Button>
    </div>
  );
}
