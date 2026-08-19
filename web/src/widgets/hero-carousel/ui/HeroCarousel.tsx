/* eslint-disable react-hooks/set-state-in-effect */
/* eslint-disable @next/next/no-img-element */
"use client";

import {
  Badge,
  Button,
  Carousel,
  type CarouselApi,
  CarouselContent,
  CarouselItem,
  CarouselNext,
  CarouselPrevious,
  Progress,
} from "@/shared/ui";
import Autoplay from "embla-carousel-autoplay";
import { useEffect, useState } from "react";

const slides = [
  {
    image:
      "https://images.unsplash.com/photo-1619466122087-e1ff06cf234b?w=1800&h=900&fit=crop&auto=format",
    badge: "LIMITED EDITION",
    title: "STAND OUT LOUD",
    description: "Bold silhouettes for those who refuse the ordinary.",
  },
  {
    image:
      "https://images.unsplash.com/photo-1554925051-f668ed70d520?w=1800&h=900&fit=crop&auto=format",
    badge: "NEW DROP",
    title: "URBAN ENERGY",
    description: "Designed for movement, crafted for the streets.",
  },
  {
    image:
      "https://images.unsplash.com/photo-1716347685367-1eb5de72eb65?w=1800&h=900&fit=crop&auto=format",
    badge: "EXCLUSIVE",
    title: "FUTURE CLASSICS",
    description: "Iconic silhouettes reimagined for a new era.",
  },
];

export function HeroCarousel() {
  const [api, setApi] = useState<CarouselApi>();
  const [current, setCurrent] = useState(0);
  const [count, setCount] = useState(0);

  const [plugin] = useState(() =>
    Autoplay({ delay: 2000, stopOnInteraction: true }),
  );

  const progress = (current * 100) / count;

  useEffect(() => {
    if (!api) {
      return;
    }

    setCount(api.scrollSnapList().length);
    setCurrent(api.selectedScrollSnap() + 1);

    api.on("select", () => {
      setCurrent(api.selectedScrollSnap() + 1);
    });
  }, [api]);

  return (
    <Carousel
      className="w-full"
      setApi={setApi}
      plugins={[plugin]}
      onMouseEnter={plugin.stop}
      onMouseLeave={plugin.reset}
    >
      <CarouselContent>
        {slides.map((slide, index) => (
          <CarouselItem key={index}>
            <div className="relative w-full h-[70vh] md:h-[85vh] overflow-hidden">
              <img
                alt={slide.title}
                className="size-full object-cover object-center"
                src={slide.image}
              />
              <div className="absolute inset-0 bg-black/40" />

              <div className="absolute inset-0 flex flex-col justify-center px-8 md:px-16 lg:px-24 max-w-4xl text-white">
                {slide.badge && <Badge className="mb-2">{slide.badge}</Badge>}
                <h1 className="text-4xl md:text-7xl font-heading uppercase tracking-wider mb-4 leading-none">
                  {slide.title}
                </h1>
                <p className="text-sm md:text-base text-gray-200 mb-8 max-w-md font-sans">
                  {slide.description}
                </p>
                <div>
                  <Button className="md:p-8 p-6 underline">EXPLORE</Button>
                </div>
              </div>
            </div>
          </CarouselItem>
        ))}
      </CarouselContent>
      <div className="absolute right-10 bottom-8 z-10 w-32 md:w-48">
        <Progress
          className="w-full bg-white/20 [&>div]:bg-white"
          value={progress}
        />
      </div>
      <CarouselPrevious className="md:absolute md:p-8 left-6 border border-white! bg-white/20! top-1/2 md:top-0" />
      <CarouselNext className="md:absolute md:p-8 right-6 border border-white! bg-white/20! top-1/2 md:top-0" />
    </Carousel>
  );
}
