"use client";

import { useDispatch, useSelector } from "react-redux";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Input, Label } from "@/shared/ui";
import type { CardData } from "@/shared/lib";
import { setCard, resetWizard } from "../model/slice";
import { CardPreview } from "./card-preview";
import { selectCard, selectFormData } from "../model/selectors";
import { cardSchema } from "../model/card-schema";
import { useRegisterMutation } from "../api/sign-up-api";
import { useRouter } from "next/navigation";

const EMPTY: CardData = { cardNumber: "", holder: "", expiry: "", cvv: "" };

const formatCardNumber = (v: string) =>
  v
    .replace(/\D/g, "")
    .slice(0, 16)
    .replace(/(.{4})/g, "$1 ")
    .trim();

const formatExpiry = (v: string) => {
  const digits = v.replace(/\D/g, "").slice(0, 4);
  if (digits.length <= 2) return digits;
  return `${digits.slice(0, 2)}/${digits.slice(2)}`;
};

const formatCvv = (v: string) => v.replace(/\D/g, "").slice(0, 4);

const formatHolder = (v: string) =>
  v.replace(/[^a-zA-Z\s'-]/g, "").toUpperCase();

export function CardStep() {
  const dispatch = useDispatch();
  const [registerUser] = useRegisterMutation();
  const formData = useSelector(selectFormData);
  const card = useSelector(selectCard) ?? EMPTY;
  const router = useRouter();

  const {
    register,
    handleSubmit,
    watch,
    setValue,
    formState: { errors },
  } = useForm<CardData>({
    resolver: zodResolver(cardSchema),
    mode: "onBlur",
    defaultValues: {
      cardNumber: card.cardNumber,
      holder: card.holder,
      expiry: card.expiry,
      cvv: card.cvv,
    },
  });

  const live = watch();

  const masked =
    (field: keyof CardData, fn: (v: string) => string) =>
    (e: React.ChangeEvent<HTMLInputElement>) =>
      setValue(field, fn(e.target.value), { shouldValidate: false });

  const onValid = async (values: CardData) => {
    dispatch(setCard(values));
    try {
      await registerUser(formData).unwrap();
      dispatch(resetWizard());
      router.push("/");
    } catch {}
  };

  return (
    <form
      id="card-form"
      onSubmit={handleSubmit(onValid)}
      className="flex flex-col gap-6"
    >
      <CardPreview card={live} />

      <div className="flex flex-col gap-2">
        <Label htmlFor="cardNumber">Card Number</Label>
        <Input
          id="cardNumber"
          inputMode="numeric"
          placeholder="4242 4242 4242 4242"
          {...register("cardNumber", {
            onChange: masked("cardNumber", formatCardNumber),
          })}
        />
        {errors.cardNumber && (
          <p className="text-sm text-destructive">
            {errors.cardNumber.message}
          </p>
        )}
      </div>

      <div className="flex flex-col gap-2">
        <Label htmlFor="holder">Cardholder Name</Label>
        <Input
          id="holder"
          placeholder="JOHN DOE"
          {...register("holder", {
            onChange: masked("holder", formatHolder),
          })}
        />
        {errors.holder && (
          <p className="text-sm text-destructive">{errors.holder.message}</p>
        )}
      </div>

      <div className="grid grid-cols-2 gap-4">
        <div className="flex flex-col gap-2">
          <Label htmlFor="expiry">Expiration Date</Label>
          <Input
            id="expiry"
            inputMode="numeric"
            placeholder="MM/YY"
            {...register("expiry", {
              onChange: masked("expiry", formatExpiry),
            })}
          />
          {errors.expiry && (
            <p className="text-sm text-destructive">{errors.expiry.message}</p>
          )}
        </div>
        <div className="flex flex-col gap-2">
          <Label htmlFor="cvv">CVV</Label>
          <Input
            id="cvv"
            inputMode="numeric"
            placeholder="•••"
            {...register("cvv", {
              onChange: masked("cvv", formatCvv),
            })}
          />
          {errors.cvv && (
            <p className="text-sm text-destructive">{errors.cvv.message}</p>
          )}
        </div>
      </div>
    </form>
  );
}
