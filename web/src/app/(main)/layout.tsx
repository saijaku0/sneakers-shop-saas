import { Container } from "@/shared/ui";
import { Header } from "@/widgets/header";

export default function MainLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <>
      <Header />
      <Container>
        <main>{children}</main>
      </Container>
    </>
  );
}
