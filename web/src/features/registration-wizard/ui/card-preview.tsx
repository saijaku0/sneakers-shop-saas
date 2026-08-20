import type { CardData } from "@/shared/lib";

export function CardPreview({ card }: { card: CardData }) {
  return (
    <div className="flex min-h-37.5 flex-col justify-between rounded-lg bg-foreground p-4 text-background">
      <div className="flex justify-between">
        <span className="text-xs uppercase tracking-widest text-background/50">
          SOLE
        </span>
        <span className="text-xs uppercase tracking-widest text-background/50">
          mock
        </span>
      </div>

      <div className="font-mono text-lg tracking-widest">
        {card.cardNumber || "•••• •••• •••• ••••"}
      </div>

      <div className="flex justify-between text-xs uppercase tracking-wide text-background/70">
        <span>{card.holder || "CARDHOLDER"}</span>
        <span>{card.expiry || "MM/YY"}</span>
      </div>
    </div>
  );
}
