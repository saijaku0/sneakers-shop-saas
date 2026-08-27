/* eslint-disable @next/next/no-img-element */
import { PackageSearch } from "lucide-react";
import { cn } from "@/shared/lib";

const MAX_VISIBLE = 3;
const OFFSET_PX = 8;

interface OrderPreviewStackProps {
  images: string[];
  alt: string;
  className?: string;
}

export function OrderPreviewStack({
  images,
  alt,
  className,
}: OrderPreviewStackProps) {
  if (images.length === 0) {
    return (
      <div
        className={cn(
          "flex size-20 shrink-0 items-center justify-center rounded-md border bg-muted text-muted-foreground",
          className,
        )}
      >
        <PackageSearch className="size-6" />
      </div>
    );
  }

  const visible = images.slice(0, MAX_VISIBLE);
  const hiddenCount = images.length - visible.length;

  return (
    <div
      className={cn("relative size-20 shrink-0", className)}
      style={{
        marginTop: (visible.length - 1) * OFFSET_PX,
        marginRight: (visible.length - 1) * OFFSET_PX,
      }}
    >
      {visible.map((src, index) => (
        <div
          key={index}
          className="absolute bottom-0 left-0 size-20 overflow-hidden rounded-md border border-border bg-card shadow-sm"
          style={{
            transform: `translate(${index * OFFSET_PX}px, ${-index * OFFSET_PX}px)`,
            zIndex: visible.length - index,
          }}
        >
          <img
            src={src}
            alt={index === 0 ? alt : ""}
            className="h-full w-full object-cover object-center"
          />
        </div>
      ))}

      {hiddenCount > 0 && (
        <span className="absolute -top-2 -right-2 z-10 flex size-6 items-center justify-center rounded-full border border-border bg-foreground text-[10px] font-semibold text-background">
          +{hiddenCount}
        </span>
      )}
    </div>
  );
}
