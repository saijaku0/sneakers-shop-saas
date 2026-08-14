"use client";

import { useMemo } from "react";
import { Menu } from "lucide-react";
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
} from "@/shared/ui";
import { buildNavigation } from "../lib/build-navigation";
import { mockNavData } from "../model/mock-nav-data";
import Link from "next/link";
import { ThemeToggle } from "@/features/theme-toggle";
import { AuthButton } from "@/features/auth-button";

export function MobileNav() {
  const navigation = useMemo(() => buildNavigation(mockNavData), []);
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
          {navigation.map((item) =>
            item.sections ? (
              <Accordion key={item.label} type="single" collapsible>
                <AccordionItem value={item.label} className="border-b-0">
                  <AccordionTrigger className="text-nav uppercase py-3">
                    {item.label}
                  </AccordionTrigger>
                  <AccordionContent className="pb-2">
                    <div className="flex flex-col gap-4 pl-3">
                      {item.sections.map((section) => (
                        <div
                          key={section.title}
                          className="flex flex-col gap-1"
                        >
                          <span className="text-xs uppercase tracking-wider text-muted-foreground">
                            {section.title}
                          </span>
                          {section.links.map((link) => (
                            <SheetClose asChild key={link.path}>
                              <Link
                                href={link.path}
                                className="py-1.5 text-sm hover:text-primary"
                              >
                                {link.label}
                              </Link>
                            </SheetClose>
                          ))}
                        </div>
                      ))}
                    </div>
                  </AccordionContent>
                </AccordionItem>
              </Accordion>
            ) : (
              <SheetClose asChild key={item.label}>
                <Link
                  href={item.path}
                  className="text-nav uppercase py-3 hover:text-primary"
                >
                  {item.label}
                </Link>
              </SheetClose>
            ),
          )}
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
