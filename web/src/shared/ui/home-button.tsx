import Link from "next/link";
import { Button } from "./button";
import { Home } from "lucide-react";

export function HomeButton() {
  return (
    <div className="mb-8">
      <Button
        variant="ghost"
        size="sm"
        className="text-muted-foreground hover:text-foreground"
        asChild
      >
        <Link href="/">
          <Home className="mr-2 h-4 w-4" />
          Home
        </Link>
      </Button>
    </div>
  );
}
