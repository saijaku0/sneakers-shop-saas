"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { useDispatch } from "react-redux";
import { LogOut } from "lucide-react";
import {
  Sidebar,
  SidebarHeader,
  SidebarContent,
  SidebarFooter,
  SidebarMenu,
  SidebarMenuItem,
  SidebarMenuButton,
} from "@/shared/ui";
import { clearToken } from "@/entities/session";

const NAV = [
  { href: "/profile/personal", label: "Personal Details" },
  { href: "/profile/orders", label: "Order History" },
  { href: "/profile/payment", label: "Payment Methods" },
];

export function ProfileSidebar() {
  const pathname = usePathname();
  const dispatch = useDispatch();

  // заглушка — потом из /viewer или профиля
  const name = "Jordan Reeves";

  return (
    <Sidebar collapsible="offcanvas">
      <SidebarHeader className="gap-3 p-6 pt-20">
        <p className="font-display text-lg font-bold uppercase tracking-tight text-foreground">
          {name}
        </p>
      </SidebarHeader>

      <SidebarContent className="p-4">
        <SidebarMenu>
          {NAV.map((item) => {
            const active = pathname === item.href;
            return (
              <SidebarMenuItem key={item.href}>
                <SidebarMenuButton
                  asChild
                  isActive={active}
                  className={
                    active
                      ? "border-l-2 border-foreground font-bold uppercase tracking-wide text-foreground"
                      : "uppercase tracking-wide text-muted-foreground"
                  }
                >
                  <Link href={item.href}>{item.label}</Link>
                </SidebarMenuButton>
              </SidebarMenuItem>
            );
          })}
        </SidebarMenu>
      </SidebarContent>

      <SidebarFooter className="p-4">
        <SidebarMenu>
          <SidebarMenuItem>
            <SidebarMenuButton
              onClick={() => dispatch(clearToken())}
              className="uppercase tracking-wide text-muted-foreground"
            >
              <LogOut className="size-4" />
              Log out
            </SidebarMenuButton>
          </SidebarMenuItem>
        </SidebarMenu>
      </SidebarFooter>
    </Sidebar>
  );
}
