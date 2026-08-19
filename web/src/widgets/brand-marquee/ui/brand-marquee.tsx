const BRANDS = [
  "Nike",
  "Adidas",
  "New Balance",
  "Jordan",
  "Puma",
  "Asics",
  "Reebok",
  "Converse",
];

export function BrandMarquee() {
  return (
    <div
      className="group relative overflow-hidden border-y border-border py-6
                 mask-[linear-gradient(to_right,transparent,black_10%,black_90%,transparent)]"
    >
      <div className="flex w-max animate-marquee motion-reduce:animate-none">
        {[...BRANDS, ...BRANDS].map((brand, i) => (
          <span
            key={i}
            aria-hidden={i >= BRANDS.length}
            className="mx-10 font-display text-4xl font-bold uppercase tracking-tight
                       text-foreground/40 transition-colors hover:text-foreground"
          >
            {brand}
          </span>
        ))}
      </div>
    </div>
  );
}
