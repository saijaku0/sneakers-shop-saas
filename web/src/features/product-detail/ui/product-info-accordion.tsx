/* eslint-disable @next/next/no-img-element */
"use client";

import {
  Accordion,
  AccordionItem,
  AccordionTrigger,
  AccordionContent,
} from "@/shared/ui";
import { ChevronDown } from "lucide-react";

const MOCK_DESCRIPTION_TITLE =
  "A sneaker with a leather and suede-overlay upper for a premium feel.";

const MOCK_DESCRIPTION_PARAGRAPHS = [
  "A timeless classic, loved since the 1950s. On the pitch or in the skatepark — make this shoe a permanent part of your wardrobe.",
  "The leather upper with suede overlays delivers a premium feel and durability. Plaid-inspired graphic details give the shoe a unique touch, while contrast stitching at the heel rounds off the look.",
  "The printed trefoil logo and the serrated 3-stripes set unmistakable accents. With its regular fit and lace closure, the shoe is a solid everyday choice.",
  "Whether you're strolling through the city streets or just hanging out with friends — this sneaker offers style and comfort in one.",
];

const MOCK_DETAILS = [
  "Regular fit",
  "Lace closure",
  "Leather upper with suede overlays",
  "Rubber outsole",
  "Printed trefoil logo",
  "Product colour: Cloud White / Blue / Gum",
];

export function ProductInfoAccordion({ image }: { image?: string }) {
  return (
    <div className="mx-auto mt-12 max-w-7xl px-4 w-full">
      <Accordion type="multiple">
        {/* Reviews (mock) */}
        <AccordionItem value="reviews" className="border-b">
          <AccordionTrigger className="group flex w-full items-center justify-between py-5 text-left">
            <span className="text-lg font-bold text-foreground">
              Reviews (5004)
            </span>
            <ChevronDown className="h-5 w-5 transition-transform group-data-[state=open]:rotate-180" />
          </AccordionTrigger>
          <AccordionContent className="pb-6 text-sm text-muted-foreground">
            <div className="flex items-center gap-2">
              <span className="font-medium text-foreground">4.8</span>
              <span>out of 5 · 5004 reviews</span>
            </div>
            <p className="mt-2">Detailed reviews coming soon.</p>
          </AccordionContent>
        </AccordionItem>

        {/* Description (mock) */}
        <AccordionItem value="description" className="border-b">
          <AccordionTrigger className="group flex w-full items-center justify-between py-5 text-left">
            <span className="text-lg font-bold text-foreground">
              Description
            </span>
            <ChevronDown className="h-5 w-5 transition-transform group-data-[state=open]:rotate-180" />
          </AccordionTrigger>
          <AccordionContent className="pb-8">
            <div className="grid gap-8 lg:grid-cols-2">
              <div>
                <h3 className="mb-4 font-display text-xl font-bold uppercase tracking-tight text-foreground">
                  {MOCK_DESCRIPTION_TITLE}
                </h3>
                <div className="flex flex-col gap-4 text-sm leading-relaxed text-muted-foreground">
                  {MOCK_DESCRIPTION_PARAGRAPHS.map((p, i) => (
                    <p key={i}>{p}</p>
                  ))}
                </div>
              </div>
              {image && (
                <div className="aspect-square overflow-hidden bg-muted">
                  <img
                    src={image}
                    alt=""
                    className="h-full w-full object-cover"
                  />
                </div>
              )}
            </div>
          </AccordionContent>
        </AccordionItem>

        {/* Details (mock) */}
        <AccordionItem value="details" className="border-b">
          <AccordionTrigger className="group flex w-full items-center justify-between py-5 text-left">
            <span className="text-lg font-bold text-foreground">Details</span>
            <ChevronDown className="h-5 w-5 transition-transform group-data-[state=open]:rotate-180" />
          </AccordionTrigger>
          <AccordionContent className="pb-6">
            <ul className="flex list-disc flex-col gap-1.5 pl-5 text-sm text-muted-foreground">
              {MOCK_DETAILS.map((d, i) => (
                <li key={i}>{d}</li>
              ))}
            </ul>
          </AccordionContent>
        </AccordionItem>
      </Accordion>
    </div>
  );
}
