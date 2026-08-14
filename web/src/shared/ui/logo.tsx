import Link from "next/link";
import { cn } from "@/shared/lib";

export function Logo({ className }: { className?: string }) {
  return (
    <Link
      href="/"
      className={cn(
        "font-heading text-2xl uppercase tracking-tight",
        className,
      )}
    >
      SOLE
    </Link>
  );
}
