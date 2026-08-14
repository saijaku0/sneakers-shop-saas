import { CartSheet } from "@/widgets/cart-sheet";
import { ThemeToggle } from "@/features/theme-toggle";
import { Container, Logo } from "@/shared/ui";
import { Search } from "lucide-react";
import { NavMenu } from "./nav-menu";
import { MobileNav } from "./mobile-nav";
import { AuthButton } from "@/features/auth-button";

export function Header() {
  return (
    <header className="py-4 bg-background border-b border-secondary">
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
