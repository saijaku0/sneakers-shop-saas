import { cn } from "@/shared/lib/utils";

export function Container({
  className,
  ...props
}: React.ComponentProps<"div">) {
  return (
    <div
      className={cn("mx-auto w-full px-4 sm:px-12 xl:px-56", className)}
      {...props}
    />
  );
}
