"use client";

import { useState } from "react";
import { Slider } from "@/shared/ui";

export function PriceSlider({
  min,
  max,
  value,
  onCommit,
}: {
  min: number;
  max: number;
  value: [number, number];
  onCommit: (v: [number, number]) => void;
}) {
  const [prevValue, setPrevValue] = useState(value);
  const [local, setLocal] = useState(value);

  // Синхронизация стейта с пропсами без useEffect (React-рекомендованный способ)
  if (value[0] !== prevValue[0] || value[1] !== prevValue[1]) {
    setPrevValue(value);
    setLocal(value);
  }

  // Защита: чтобы ползунок не ломался, если в базе все товары стоят одинаково
  const safeMax = Math.max(min + 1, max);

  return (
    <div className="flex flex-col gap-3">
      <Slider
        min={min}
        max={safeMax}
        step={1}
        value={local}
        onValueChange={(v) => setLocal(v as [number, number])}
        onValueCommit={(v) => onCommit(v as [number, number])}
      />
      <div className="flex justify-between text-sm text-muted-foreground">
        <span>€ {local[0]}</span>
        <span>€ {local[1]}</span>
      </div>
    </div>
  );
}
