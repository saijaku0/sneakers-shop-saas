"use client";

import { useDispatch, useSelector } from "react-redux";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Input, Label } from "@/shared/ui";
import type { DefaultShippingAddress } from "@/shared/lib";
import { updateData, nextStep } from "../model/slice";
import { selectFormData } from "../model/selectors";
import { addressSchema } from "../model/address-schema";

const EMPTY: DefaultShippingAddress = {
  country: "",
  state: "",
  city: "",
  street: "",
  houseNumber: "",
  zipCode: "",
};

export function AddressStep() {
  const dispatch = useDispatch();
  const data = useSelector(selectFormData);
  const address = data.defaultShippingAddress ?? EMPTY;

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<DefaultShippingAddress>({
    resolver: zodResolver(addressSchema),
    mode: "onBlur",
    defaultValues: {
      country: address.country,
      state: address.state ?? "",
      city: address.city,
      street: address.street,
      houseNumber: address.houseNumber,
      zipCode: address.zipCode,
    },
  });

  const onValid = (values: DefaultShippingAddress) => {
    dispatch(updateData({ defaultShippingAddress: values }));
    dispatch(nextStep());
  };

  return (
    <form
      id="address-form"
      onSubmit={handleSubmit(onValid)}
      className="flex flex-col gap-6"
    >
      <div className="grid grid-cols-2 gap-4">
        <div className="flex flex-col gap-2">
          <Label htmlFor="country">Country</Label>
          <Input id="country" placeholder="Germany" {...register("country")} />
          {errors.country && (
            <p className="text-sm text-destructive">{errors.country.message}</p>
          )}
        </div>
        <div className="flex flex-col gap-2">
          <Label htmlFor="state">State</Label>
          <Input id="state" placeholder="Bavaria" {...register("state")} />
          {errors.state && (
            <p className="text-sm text-destructive">{errors.state.message}</p>
          )}
        </div>
      </div>

      <div className="grid grid-cols-2 gap-4">
        <div className="flex flex-col gap-2">
          <Label htmlFor="city">City</Label>
          <Input id="city" placeholder="Nuremberg" {...register("city")} />
          {errors.city && (
            <p className="text-sm text-destructive">{errors.city.message}</p>
          )}
        </div>
        <div className="flex flex-col gap-2">
          <Label htmlFor="zipCode">ZIP Code</Label>
          <Input id="zipCode" placeholder="90402" {...register("zipCode")} />
          {errors.zipCode && (
            <p className="text-sm text-destructive">{errors.zipCode.message}</p>
          )}
        </div>
      </div>

      <div className="grid grid-cols-[1fr_120px] gap-4">
        <div className="flex flex-col gap-2">
          <Label htmlFor="street">Street</Label>
          <Input
            id="street"
            placeholder="Königstraße"
            {...register("street")}
          />
          {errors.street && (
            <p className="text-sm text-destructive">{errors.street.message}</p>
          )}
        </div>
        <div className="flex flex-col gap-2">
          <Label htmlFor="houseNumber">House Number</Label>
          <Input
            id="houseNumber"
            placeholder="12"
            {...register("houseNumber")}
          />
          {errors.houseNumber && (
            <p className="text-sm text-destructive">
              {errors.houseNumber.message}
            </p>
          )}
        </div>
      </div>
    </form>
  );
}
