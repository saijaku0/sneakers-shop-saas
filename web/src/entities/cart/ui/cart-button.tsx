import { ShoppingCart } from "lucide-react";
import { Button } from "@/shared/ui";
import { cn } from "@/shared/lib/utils";

interface CartButtonProps extends React.ComponentProps<typeof Button> {
  count?: number;
}

export function CartButton({
  count = 0,
  className,
  ...props
}: CartButtonProps) {
  const hasItems = count > 0;
  return (
    <Button
      variant="outline"
      size="icon"
      aria-label={hasItems ? `Cart, ${count} items` : "Cart, empty"}
      className={cn("relative cursor-pointer", className)}
      {...props}
    >
      <ShoppingCart className="size-6" />
      {hasItems && (
        <span className="absolute -right-2 -top-2 flex h-5 min-w-5 items-center justify-center rounded-full bg-primary px-1 text-[10px] font-bold text-primary-foreground">
          {count > 99 ? "99+" : count}
        </span>
      )}
    </Button>
  );
}
