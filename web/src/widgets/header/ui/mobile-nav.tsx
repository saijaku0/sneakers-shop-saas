"use client";

import Link from "next/link";
import { Menu } from "lucide-react";
import { useMemo } from "react";

import {
  Accordion,
  AccordionContent,
  AccordionItem,
  AccordionTrigger,
  Button,
  Sheet,
  SheetClose,
  SheetContent,
  SheetHeader,
  SheetTitle,
  SheetTrigger,
  Skeleton,
} from "@/shared/ui";
import { ThemeToggle } from "@/features/theme-toggle";
import { AuthButton } from "@/features/auth-button";
import { useGetCategoriesQuery } from "@/entities/category";
import { useGetBrandsQuery } from "@/entities/brand";

import { adaptNavData } from "../lib/adapt-nav-data";
import { buildNavigation } from "../lib/build-navigation";

function NavigationSkeleton() {
  return (
    <div className="flex flex-col gap-4 py-2">
      {Array.from({ length: 3 }).map((_, index) => (
        <Skeleton key={index} className="h-10 w-full" />
      ))}
    </div>
  );
}

function NavigationSection({
  title,
  links,
}: {
  title: string;
  links: Array<{ label: string; path: string }>;
}) {
  return (
    <div className="flex flex-col gap-1">
      <span className="text-xs uppercase tracking-wider text-muted-foreground">
        {title}
      </span>

      {links.map((link) => (
        <SheetClose asChild key={link.path}>
          <Link href={link.path} className="py-1.5 text-sm hover:text-primary">
            {link.label}
          </Link>
        </SheetClose>
      ))}
    </div>
  );
}

function NavigationItem({
  item,
}: {
  item: ReturnType<typeof buildNavigation>[number];
}) {
  if (!item.sections) {
    return (
      <SheetClose asChild>
        <Link
          href={item.path}
          className="py-3 text-nav uppercase hover:text-primary"
        >
          {item.label}
        </Link>
      </SheetClose>
    );
  }

  return (
    <Accordion type="single" collapsible>
      <AccordionItem value={item.label} className="border-b-0">
        <AccordionTrigger className="py-3 text-nav uppercase">
          {item.label}
        </AccordionTrigger>

        <AccordionContent className="pb-2">
          <div className="flex flex-col gap-4 pl-3">
            {item.sections.map((section) => (
              <NavigationSection
                key={section.title}
                title={section.title}
                links={section.links}
              />
            ))}
          </div>
        </AccordionContent>
      </AccordionItem>
    </Accordion>
  );
}

function MobileNavigation({
  navigation,
  isLoading,
}: {
  navigation: ReturnType<typeof buildNavigation>;
  isLoading: boolean;
}) {
  if (isLoading) {
    return <NavigationSkeleton />;
  }

  return (
    <>
      {navigation.map((item) => (
        <NavigationItem key={item.label} item={item} />
      ))}
    </>
  );
}

export function MobileNav() {
  const { data: categories, isLoading: isCategoriesLoading } =
    useGetCategoriesQuery();

  const { data: brands, isLoading: isBrandsLoading } = useGetBrandsQuery();

  const navigation = useMemo(() => {
    const navData = adaptNavData(categories, brands);

    return buildNavigation(navData);
  }, [categories, brands]);

  const isLoading = isCategoriesLoading || isBrandsLoading;

  return (
    <Sheet>
      <SheetTrigger asChild>
        <Button
          variant="ghost"
          size="icon"
          aria-label="Open menu"
          className="lg:hidden"
        >
          <Menu className="size-6" />
        </Button>
      </SheetTrigger>

      <SheetContent side="left" className="w-80">
        <SheetHeader>
          <SheetTitle className="font-heading text-2xl">SOLE</SheetTitle>
        </SheetHeader>

        <nav className="flex flex-col px-2 py-4">
          <MobileNavigation navigation={navigation} isLoading={isLoading} />
        </nav>

        <div className="mt-auto flex flex-col gap-4 border-t px-4 py-4">
          <AuthButton orientation="col" />

          <div className="flex items-center justify-between">
            <span className="text-sm text-muted-foreground">Account</span>
            <ThemeToggle />
          </div>
        </div>
      </SheetContent>
    </Sheet>
  );
}
