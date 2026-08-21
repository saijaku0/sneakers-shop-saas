"use client";

import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/shared/ui";
import { useFilters } from "../lib/use-filters";
import { useUpdateQuery } from "../lib/use-update-query";

const SORT_OPTIONS = [
  { value: "newest", label: "Newest" },
  { value: "price_asc", label: "Price: Low to High" },
  { value: "price_desc", label: "Price: High to Low" },
  { value: "name", label: "Name" },
];

export function SortSelect() {
  const filters = useFilters();
  const updateQuery = useUpdateQuery();

  return (
    <div className="flex items-center gap-2">
      <span className="hidden text-sm text-muted-foreground sm:inline">
        Sort by
      </span>
      <Select
        value={filters.sortBy}
        onValueChange={(value) =>
          updateQuery({ sortBy: value === "newest" ? null : value, page: null })
        }
      >
        <SelectTrigger className="w-44 rounded-none border-foreground/20 focus:ring-0 focus:ring-offset-0">
          <SelectValue />
        </SelectTrigger>
        <SelectContent className="rounded-none">
          {SORT_OPTIONS.map((o) => (
            <SelectItem key={o.value} value={o.value} className="rounded-none">
              {o.label}
            </SelectItem>
          ))}
        </SelectContent>
      </Select>
    </div>
  );
}
