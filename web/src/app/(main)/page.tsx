import { Container } from "@/shared/ui";
import { BrandMarquee } from "@/widgets/brand-marquee";
import { HeroCarousel } from "@/widgets/hero-carousel";
import { NewArrivial } from "@/widgets/new-arrivals";

export default function HomePage() {
  return (
    <>
      <section id="carousel">
        <HeroCarousel />
      </section>
      <section id="marquee">
        <BrandMarquee />
      </section>
      <Container>
        <section>
          <NewArrivial />
        </section>
      </Container>
    </>
  );
}
