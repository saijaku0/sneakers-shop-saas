import { AlertCircle, Minus, Plus, Trash2 } from "lucide-react";
import { Button } from "@/shared/ui";
import { cn } from "@/shared/lib/utils";
import type { CartItem as CartItemType } from "../model/types";

interface CartItemProps {
  item: CartItemType;
  onQuantityChange: (id: string, delta: number) => void;
  onRemove: (id: string) => void;
}

export function CartItem({ item, onQuantityChange, onRemove }: CartItemProps) {
  return (
    <div
      className={cn("flex items-start gap-4", !item.inStock && "opacity-60")}
    >
      <div className="h-20 w-20 shrink-0 overflow-hidden rounded-md border bg-muted">
        <img
          src={item.image.url}
          alt={item.image.alt}
          className="h-full w-full object-cover object-center"
        />
      </div>

      <div className="flex flex-1 flex-col">
        <span className="text-xs uppercase tracking-wider text-muted-foreground">
          {item.brand}
        </span>
        <span className="line-clamp-1 text-sm font-semibold">{item.name}</span>
        <span className="mt-0.5 text-xs text-muted-foreground">
          Size: {item.size} | Color: {item.color}
        </span>

        <div className="mt-2 flex items-center justify-between">
          {item.inStock ? (
            <span className="text-sm font-medium">
              ${item.unitPrice.toFixed(2)}
            </span>
          ) : (
            <span className="flex items-center text-xs font-medium text-destructive">
              <AlertCircle className="mr-1 h-3 w-3" /> Out of stock
            </span>
          )}
        </div>

        <div className="mt-3 flex items-center justify-between">
          <div className="flex items-center rounded-md border">
            <Button
              variant="ghost"
              size="icon"
              className="h-7 w-7 rounded-none"
              onClick={() => onQuantityChange(item.warehouseItemId, -1)}
              disabled={!item.inStock || item.quantity <= 1}
            >
              <Minus className="h-3 w-3" />
            </Button>
            <span className="w-8 text-center text-sm font-medium">
              {item.quantity}
            </span>
            <Button
              variant="ghost"
              size="icon"
              className="h-7 w-7 rounded-none"
              onClick={() => onQuantityChange(item.warehouseItemId, 1)}
              disabled={!item.inStock || item.quantity >= item.maxAvailable}
            >
              <Plus className="h-3 w-3" />
            </Button>
          </div>
          <Button
            variant="ghost"
            size="icon"
            className="h-7 w-7 text-muted-foreground hover:text-destructive"
            onClick={() => onRemove(item.warehouseItemId)}
          >
            <Trash2 className="h-4 w-4" />
          </Button>
        </div>
      </div>
    </div>
  );
}
