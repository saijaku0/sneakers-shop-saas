"use client";

import { Label, RadioGroup, RadioGroupItem } from "@/shared/ui";
import { cn } from "@/shared/lib";
import { PaymentMethod } from "@/entities/order";

const PAYMENT_METHOD_OPTIONS: {
  value: PaymentMethod;
  title: string;
  description: string;
}[] = [
  {
    value: PaymentMethod.CreditCard,
    title: "Credit Card",
    description: "Pay securely with your credit card",
  },
  {
    value: PaymentMethod.DebitCard,
    title: "Debit Card",
    description: "Pay directly from your bank account",
  },
  {
    value: PaymentMethod.PayPal,
    title: "PayPal",
    description: "Fast and secure checkout with PayPal",
  },
  {
    value: PaymentMethod.Cash,
    title: "Cash",
    description: "Pay on delivery",
  },
];

interface PaymentStepProps {
  paymentMethod: PaymentMethod;
  onPaymentMethodChange: (method: PaymentMethod) => void;
  onSubmit: () => void;
  formId: string;
}

export function PaymentStep({
  paymentMethod,
  onPaymentMethodChange,
  onSubmit,
  formId,
}: PaymentStepProps) {
  return (
    <form
      id={formId}
      onSubmit={(e) => {
        e.preventDefault();
        onSubmit();
      }}
      className="flex flex-col gap-3"
    >
      <h3 className="text-sm font-semibold uppercase tracking-wide">
        Payment Method
      </h3>

      <RadioGroup
        value={String(paymentMethod)}
        onValueChange={(value) =>
          onPaymentMethodChange(Number(value) as PaymentMethod)
        }
        className="gap-3"
      >
        {PAYMENT_METHOD_OPTIONS.map((option) => {
          const isSelected = paymentMethod === option.value;
          const id = `payment-method-${option.value}`;

          return (
            <Label
              key={option.value}
              htmlFor={id}
              className={cn(
                "flex cursor-pointer items-center gap-3 rounded-md border bg-card p-4 font-normal transition-colors",
                isSelected ? "border-primary" : "hover:border-foreground/30",
              )}
            >
              <RadioGroupItem value={String(option.value)} id={id} />
              <div className="flex flex-col">
                <span className="text-sm font-medium text-foreground">
                  {option.title}
                </span>
                <span className="text-xs text-muted-foreground">
                  {option.description}
                </span>
              </div>
            </Label>
          );
        })}
      </RadioGroup>
    </form>
  );
}
