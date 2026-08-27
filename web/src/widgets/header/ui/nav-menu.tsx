"use client";

import { useMemo } from "react";
import { NavigationMenu, NavigationMenuList, Skeleton } from "@/shared/ui";
import { NavMenuItem } from "./nav-menu-item";
import { buildNavigation } from "../lib/build-navigation";
import { cn } from "@/shared/lib";
import { useGetCategoriesQuery } from "@/entities/category";
import { useGetBrandsQuery } from "@/entities/brand";
import { adaptNavData } from "../lib/adapt-nav-data";

export function NavMenu({
  className,
  ...props
}: React.ComponentProps<typeof NavigationMenu>) {
  const { data: rawCategories, isLoading: isCategoriesLoading } =
    useGetCategoriesQuery();
  const { data: rawBrands, isLoading: isBrandsLoading } = useGetBrandsQuery();
  const navData = useMemo(() => {
    return adaptNavData(rawCategories, rawBrands);
  }, [rawCategories, rawBrands]);
  const navigation = useMemo(() => {
    return buildNavigation(navData);
  }, [navData]);

  if (isCategoriesLoading || isBrandsLoading) {
    return (
      <div className={cn("flex items-center gap-6", className)}>
        <Skeleton className="h-5 w-16" />
        <Skeleton className="h-5 w-20" />
        <Skeleton className="h-5 w-14" />
        <Skeleton className="h-5 w-24" />
      </div>
    );
  }

  return (
    <NavigationMenu
      className={cn("flex w-full flex-col", className)}
      {...props}
    >
      <NavigationMenuList className="xl:gap-x-8 xl:font-header">
        {navigation.map((item) => (
          <NavMenuItem key={item.path} item={item} />
        ))}
      </NavigationMenuList>
    </NavigationMenu>
  );
}
