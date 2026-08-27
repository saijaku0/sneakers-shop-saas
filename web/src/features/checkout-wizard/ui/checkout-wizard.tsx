"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { useSelector } from "react-redux";
import { Button, StepProgress } from "@/shared/ui";
import type { DefaultShippingAddress } from "@/shared/lib";
import { selectCartItems, useGetCartQuery } from "@/entities/cart";
import { PaymentMethod, useCreateOrderMutation } from "@/entities/order";
import { CHECKOUT_STEPS } from "../model/constants";
import { CartStep } from "./cart-step";
import { ShippingStep } from "./shipping-step";
import { PaymentStep } from "./payment-step";
import { ConfirmStep } from "./confirm-step";
import { OrderSummary } from "./order-summary";

const FORM_IDS = ["cart-step", "shipping-form", "payment-form", "confirm-step"];

export function CheckoutWizard() {
  const router = useRouter();
  const [step, setStep] = useState(1);

  const items = useSelector(selectCartItems);
  const { isLoading: isCartLoading } = useGetCartQuery();

  const [address, setAddress] = useState<DefaultShippingAddress | null>(null);
  const [paymentMethod, setPaymentMethod] = useState<PaymentMethod>(
    PaymentMethod.CreditCard,
  );

  const [createOrder, { isLoading: isPlacingOrder }] = useCreateOrderMutation();
  const [placeOrderError, setPlaceOrderError] = useState<string | null>(null);

  const handleBack = () => setStep((s) => Math.max(1, s - 1));
  const handleNext = () =>
    setStep((s) => Math.min(CHECKOUT_STEPS.length, s + 1));

  const handlePlaceOrder = async () => {
    if (!address) return;

    setPlaceOrderError(null);
    try {
      const orderId = await createOrder({
        shippingAddress: address,
        paymentMethod,
      }).unwrap();
      router.push(`/orders/${orderId}`);
    } catch (err) {
      console.error("Failed to place order:", err);
      setPlaceOrderError("Failed to place your order. Please try again.");
    }
  };

  return (
    <div className="mx-auto w-full max-w-350 px-4 py-8 sm:px-6 lg:px-8">
      <div className="mb-10">
        <StepProgress currentStep={step} labels={CHECKOUT_STEPS} />
      </div>

      <div className="grid grid-cols-1 gap-8 lg:grid-cols-[1fr_360px]">
        <div className="flex min-w-0 flex-col">
          <h2 className="mb-6 font-display text-3xl font-bold uppercase tracking-tight text-foreground">
            {CHECKOUT_STEPS[step - 1]}
          </h2>

          {step === 1 && <CartStep />}

          {step === 2 && (
            <ShippingStep
              onAddressChange={setAddress}
              onSubmit={handleNext}
              formId={FORM_IDS[1]}
            />
          )}

          {step === 3 && (
            <PaymentStep
              paymentMethod={paymentMethod}
              onPaymentMethodChange={setPaymentMethod}
              onSubmit={handleNext}
              formId={FORM_IDS[2]}
            />
          )}

          {step === 4 && (
            <ConfirmStep
              items={items}
              address={address}
              paymentMethod={paymentMethod}
            />
          )}

          {step === 4 && placeOrderError && (
            <p className="mt-4 text-sm text-destructive">{placeOrderError}</p>
          )}

          <div className="mt-8 flex items-center gap-3">
            {step > 1 && (
              <Button
                variant="outline"
                size="lg"
                className="flex-1"
                onClick={handleBack}
                disabled={isPlacingOrder}
              >
                Back
              </Button>
            )}

            {step === 1 && (
              <Button
                size="lg"
                className="flex-1"
                disabled={isCartLoading || items.length === 0}
                onClick={handleNext}
              >
                Continue to Shipping
              </Button>
            )}

            {(step === 2 || step === 3) && (
              <Button
                size="lg"
                className="flex-1"
                type="submit"
                form={FORM_IDS[step - 1]}
              >
                {step === 2 ? "Continue to Payment" : "Review Order"}
              </Button>
            )}

            {step === 4 && (
              <Button
                size="lg"
                className="flex-1"
                onClick={handlePlaceOrder}
                disabled={!address || isPlacingOrder}
              >
                {isPlacingOrder ? "Placing Order…" : "Place Order"}
              </Button>
            )}
          </div>
        </div>

        <div className="lg:sticky lg:top-24 lg:self-start">
          <OrderSummary items={items} />
        </div>
      </div>
    </div>
  );
}
