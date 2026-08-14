"use client";

import { cn } from "@/shared/lib";
import {
  Menubar,
  MenubarContent,
  MenubarMenu,
  MenubarRadioGroup,
  MenubarRadioItem,
  MenubarTrigger,
} from "@/shared/ui";
import { useTheme } from "next-themes";

export function ThemeToggle({
  className,
  ...props
}: React.ComponentProps<typeof Menubar>) {
  const { theme, setTheme } = useTheme();
  return (
    <Menubar className={cn(className)} {...props}>
      <MenubarMenu>
        <MenubarTrigger className="xl:text-header!">Theme</MenubarTrigger>
        <MenubarContent>
          <MenubarRadioGroup value={theme} onValueChange={setTheme}>
            <MenubarRadioItem value="light">Light</MenubarRadioItem>
            <MenubarRadioItem value="dark">Dark</MenubarRadioItem>
            <MenubarRadioItem value="system">System</MenubarRadioItem>
          </MenubarRadioGroup>
        </MenubarContent>
      </MenubarMenu>
    </Menubar>
  );
}
