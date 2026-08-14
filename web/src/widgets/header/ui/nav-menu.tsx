"use client";

import { useMemo } from "react";
import { NavigationMenu, NavigationMenuList } from "@/shared/ui";
import { NavMenuItem } from "./nav-menu-item";
import { mockNavData } from "../model/mock-nav-data";
import { buildNavigation } from "../lib/build-navigation";
import { cn } from "@/shared/lib";

export function NavMenu({
  className,
  ...props
}: React.ComponentProps<typeof NavigationMenu>) {
  const navigation = useMemo(() => buildNavigation(mockNavData), []);

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
