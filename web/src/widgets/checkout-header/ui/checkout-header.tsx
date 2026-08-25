import { Container, Logo } from "@/shared/ui";
import { CartSheet } from "@/widgets/cart-sheet";

export function CheckoutHeader() {
  return (
    <header className="py-4 border-b-2">
      <Container className="flex justify-between items-center">
        <Logo />
        <CartSheet />
      </Container>
    </header>
  );
}
