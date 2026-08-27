import { CheckoutHeader } from "@/widgets/checkout";

export default function OrdersLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <>
      <CheckoutHeader />
      {children}
    </>
  );
}
