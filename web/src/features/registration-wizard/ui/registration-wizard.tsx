"use client";

import { useDispatch, useSelector } from "react-redux";
import { Button, HomeButton, StepProgress } from "@/shared/ui";
import { AccountStep } from "./account-step";
import { AddressStep } from "./address-step";
import { CardStep } from "./card-step";
import { selectStep } from "../model/selectors";
import { nextStep, prevStep } from "../model/slice";

const TITLES = ["Create Account", "Shipping Address", "Payment Method"];
const TOTAL_STEPS = 3;
const FORM_IDS = ["account-form", "address-form", "card-form"];

export function RegistrationWizard() {
  const dispatch = useDispatch();
  const step = useSelector(selectStep);

  const handlePrev = () => dispatch(prevStep());
  const handleSkip = () => dispatch(nextStep());

  return (
    <div className="mx-auto w-full max-w-lg bg-background px-8 py-10">
      <HomeButton />
      <StepProgress currentStep={step} />

      <h2 className="mt-10 mb-8 font-display text-3xl font-bold uppercase tracking-tight text-foreground">
        {TITLES[step - 1]}
      </h2>

      {step === 1 && <AccountStep />}
      {step === 2 && <AddressStep />}
      {step === 3 && <CardStep />}

      <div className="mt-10 flex items-center gap-3">
        {step > 1 && (
          <Button
            variant="outline"
            size="lg"
            className="flex-1"
            onClick={handlePrev}
          >
            Back
          </Button>
        )}

        {step === 2 && (
          <Button
            variant="ghost"
            size="lg"
            className="flex-1"
            onClick={handleSkip}
          >
            Skip
          </Button>
        )}

        <Button
          size="lg"
          className="flex-1"
          type="submit"
          form={FORM_IDS[step - 1]}
        >
          {step < TOTAL_STEPS ? "Next" : "Register"}
        </Button>
      </div>
    </div>
  );
}
