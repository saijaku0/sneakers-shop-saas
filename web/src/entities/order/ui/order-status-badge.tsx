import { Badge } from "@/shared/ui";
import { cn } from "@/shared/lib";

const STATUS_VARIANT: Record<
  string,
  "default" | "secondary" | "destructive" | "outline"
> = {
  Pending: "outline",
  Paid: "default",
  Packaging: "secondary",
  Shipping: "secondary",
  Delivered: "default",
  Cancelled: "destructive",
};

const STATUS_DOT: Record<string, string> = {
  Pending: "bg-muted-foreground",
  Paid: "bg-primary-foreground",
  Packaging: "bg-muted-foreground",
  Shipping: "bg-muted-foreground",
  Delivered: "bg-primary-foreground",
  Cancelled: "bg-destructive",
};

export function OrderStatusBadge({
  status,
  className,
}: {
  status: string;
  className?: string;
}) {
  return (
    <Badge
      variant={STATUS_VARIANT[status] ?? "outline"}
      className={cn(
        "h-6 gap-1.5 px-2.5 text-xs uppercase tracking-wide",
        className,
      )}
    >
      <span
        className={cn(
          "size-1.5 shrink-0 rounded-full",
          STATUS_DOT[status] ?? "bg-muted-foreground",
        )}
      />
      {status}
    </Badge>
  );
}
