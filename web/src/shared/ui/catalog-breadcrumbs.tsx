"use client";

import Link from "next/link";
import { useRouter } from "next/navigation";
import { CornerUpLeft } from "lucide-react";

interface CatalogBreadcrumbsProps {
  gender?: string;
  category?: string;
}

const interactiveClassName =
  "underline hover:bg-foreground hover:text-background transition-colors";

export function CatalogBreadcrumbs({
  gender,
  category,
}: CatalogBreadcrumbsProps) {
  const router = useRouter();

  return (
    <nav className="mb-6 flex items-center gap-4 text-base text-foreground">
      <button
        type="button"
        onClick={() => router.back()}
        className={`flex items-center gap-1 font-bold ${interactiveClassName}`}
      >
        <CornerUpLeft className="h-4 w-4" strokeWidth={2} />
        Back
      </button>

      <div className="flex items-center gap-2">
        <Link href="/" className={interactiveClassName}>
          Home
        </Link>

        {gender && (
          <>
            <span>/</span>

            <Link
              href={`/${gender.toLowerCase()}`}
              className={interactiveClassName}
            >
              {gender}
            </Link>
          </>
        )}

        {category && (
          <>
            <span>/</span>
            <span className="capitalize">{category}</span>
          </>
        )}
      </div>
    </nav>
  );
}
