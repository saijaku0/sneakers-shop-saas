import Link from "next/link";
import {
  NavigationMenuContent,
  NavigationMenuItem,
  NavigationMenuLink,
  NavigationMenuTrigger,
  navigationMenuTriggerStyle,
} from "@/shared/ui";
import { cn } from "@/shared/lib";
import { NavItem } from "../model/types";

export function NavMenuItem({ item }: { item: NavItem }) {
  if (!item.sections) {
    return (
      <NavigationMenuItem>
        <NavigationMenuLink
          asChild
          className={cn(navigationMenuTriggerStyle(), "text-sale")}
        >
          <Link href={item.path}>{item.label}</Link>
        </NavigationMenuLink>
      </NavigationMenuItem>
    );
  }

  return (
    <NavigationMenuItem>
      <NavigationMenuTrigger className="cursor-pointer">
        {item.label}
      </NavigationMenuTrigger>
      <NavigationMenuContent>
        <div className="grid w-100 gap-3 p-4 md:w-125 md:grid-cols-2 lg:w-150">
          <NavigationMenuLink asChild>
            <Link
              href={item.path}
              className="col-span-full font-medium text-primary hover:underline"
            >
              View all {item.label} →
            </Link>
          </NavigationMenuLink>

          {item.sections.map((section) => (
            <div key={section.title} className="flex flex-col">
              <h4 className="py-4 font-medium uppercase leading-none text-muted-foreground lg:text-xl">
                {section.title}
              </h4>
              <ul className="flex flex-col space-y-1">
                {section.links.map((link) => (
                  <li key={link.path}>
                    <NavigationMenuLink asChild>
                      <Link
                        href={link.path}
                        className="block select-none rounded-md p-2 font-medium leading-none no-underline outline-none transition-colors hover:bg-accent hover:text-accent-foreground focus:bg-accent focus:text-accent-foreground"
                      >
                        {link.label}
                      </Link>
                    </NavigationMenuLink>
                  </li>
                ))}
              </ul>
            </div>
          ))}
        </div>
      </NavigationMenuContent>
    </NavigationMenuItem>
  );
}
