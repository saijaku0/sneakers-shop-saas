import Link from "next/link";
import { Button } from "@/shared/ui";

export default function NotFound() {
  return (
    <div className="flex min-h-screen flex-col items-center justify-center gap-6 px-4 text-center">
      <p className="font-display text-8xl font-bold uppercase tracking-tight text-foreground">
        404
      </p>
      <div className="flex flex-col gap-2">
        <h1 className="font-display text-2xl font-bold uppercase tracking-tight text-foreground">
          Page not found
        </h1>
        <p className="text-muted-foreground">
          The page you're looking for doesn't exist or has been moved.
        </p>
      </div>
      <Button asChild size="lg">
        <Link href="/">Back to home</Link>
      </Button>
    </div>
  );
}
