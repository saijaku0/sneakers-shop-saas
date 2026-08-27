import { CheckoutHeader } from "@/widgets/checkout";

export default function ChechkoutLayout({
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
