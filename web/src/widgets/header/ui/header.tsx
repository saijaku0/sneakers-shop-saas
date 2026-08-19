"use client";

import { CartSheet } from "@/widgets/cart-sheet";
import { ThemeToggle } from "@/features/theme-toggle";
import { Container, Logo } from "@/shared/ui";
import { Search } from "lucide-react";
import { NavMenu } from "./nav-menu";
import { MobileNav } from "./mobile-nav";
import { AuthButton } from "@/features/auth-button";
import { useEffect, useState } from "react";

export function Header() {
  const [isScrolled, setIsScrolled] = useState(false);

  useEffect(() => {
    const handleScroll = () => {
      if (window.scrollY > 20) {
        setIsScrolled(true);
      } else {
        setIsScrolled(false);
      }
    };

    window.addEventListener("scroll", handleScroll);
    return () => window.removeEventListener("scroll", handleScroll);
  }, []);

  return (
    <header
      className={`fixed top-0 left-0 w-full z-40 transition-all duration-300 py-4 ${
        isScrolled
          ? "bg-background border-b border-border shadow-sm"
          : "border-b border-transparent bg-background/20 backdrop-blur-md "
      }`}
    >
      <Container className="flex justify-between items-center">
        <div className="flex items-center gap-2">
          <MobileNav />
          <Logo />
        </div>

        <NavMenu className="hidden lg:flex" />
        <div className="flex items-center gap-2">
          <Search className="lg:w-6 lg:h-6 cursor-pointer" />
          <ThemeToggle className="hidden lg:inline-flex" />
          <div className="hidden lg:flex">
            <AuthButton />
          </div>
          <CartSheet />
        </div>
      </Container>
    </header>
  );
}
