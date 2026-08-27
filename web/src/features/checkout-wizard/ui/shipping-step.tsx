"use client";

import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import {
  Checkbox,
  Input,
  Label,
  RadioGroup,
  RadioGroupItem,
  Skeleton,
} from "@/shared/ui";
import { cn } from "@/shared/lib";
import type { DefaultShippingAddress } from "@/shared/lib";
import {
  addressSchema,
  useGetAddressQuery,
  useUpdateAddressMutation,
} from "@/entities/profile";

const EMPTY_ADDRESS: DefaultShippingAddress = {
  country: "",
  state: "",
  city: "",
  street: "",
  houseNumber: "",
  zipCode: "",
};

type AddressMode = "default" | "different";

interface ShippingStepProps {
  onAddressChange: (address: DefaultShippingAddress) => void;
  onSubmit: () => void;
  formId: string;
}

export function ShippingStep({
  onAddressChange,
  onSubmit,
  formId,
}: ShippingStepProps) {
  const { data, isLoading, isError } = useGetAddressQuery();
  const [updateAddress, { isLoading: isSaving }] = useUpdateAddressMutation();
  const [saveError, setSaveError] = useState<string | null>(null);
  const [requestedMode, setRequestedMode] = useState<AddressMode>("default");
  const [setAsDefault, setSetAsDefault] = useState(false);

  const defaultAddress = data?.address ?? null;
  const hasDefaultAddress = Boolean(defaultAddress);
  const mode: AddressMode = hasDefaultAddress ? requestedMode : "different";

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<DefaultShippingAddress>({
    resolver: zodResolver(addressSchema),
    mode: "onBlur",
    defaultValues: EMPTY_ADDRESS,
  });

  useEffect(() => {
    if (defaultAddress) {
      reset(defaultAddress);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [defaultAddress]);

  const handleUseDefault = () => {
    if (!defaultAddress) return;
    setSaveError(null);
    onAddressChange(defaultAddress);
    onSubmit();
  };

  const onValidDifferent = async (values: DefaultShippingAddress) => {
    const address: DefaultShippingAddress = {
      ...values,
      state: values.state?.trim() ? values.state.trim() : null,
    };

    setSaveError(null);

    if (setAsDefault) {
      try {
        await updateAddress({ address }).unwrap();
      } catch (err) {
        console.error("Failed to update address:", err);
        setSaveError("Couldn't save your address. Please try again.");
        return;
      }
    }

    onAddressChange(address);
    onSubmit();
  };

  if (isLoading) {
    return (
      <div className="flex flex-col gap-4">
        <Skeleton className="h-4 w-40" />
        <Skeleton className="h-20 w-full" />
        <Skeleton className="h-20 w-full" />
      </div>
    );
  }

  return (
    <form
      id={formId}
      onSubmit={(e) => {
        if (mode === "default") {
          e.preventDefault();
          handleUseDefault();
        } else {
          handleSubmit(onValidDifferent)(e);
        }
      }}
      className="flex flex-col gap-6"
    >
      <div className="flex flex-col gap-4">
        <h3 className="text-sm font-semibold uppercase tracking-wide">
          Shipping Address
        </h3>

        {isError && (
          <p className="text-sm text-muted-foreground">
            Couldn&apos;t load your saved address — please enter a new one.
          </p>
        )}

        <RadioGroup
          value={mode}
          onValueChange={(value) => setRequestedMode(value as AddressMode)}
          className="gap-3"
        >
          <Label
            htmlFor="address-mode-default"
            className={cn(
              "flex flex-col items-start gap-3 rounded-md border bg-card p-4 font-normal transition-colors",
              !hasDefaultAddress && "cursor-not-allowed opacity-50",
              hasDefaultAddress && mode === "default" && "border-primary",
              hasDefaultAddress &&
                mode !== "default" &&
                "hover:border-foreground/30",
            )}
          >
            <div className="flex items-center gap-3">
              <RadioGroupItem
                value="default"
                id="address-mode-default"
                disabled={!hasDefaultAddress}
              />
              <span className="text-sm font-medium text-foreground">
                Use default address
              </span>
            </div>

            {defaultAddress ? (
              <div className="pl-7 text-sm text-muted-foreground">
                <p className="text-foreground">
                  {defaultAddress.street} {defaultAddress.houseNumber}
                </p>
                <p>
                  {defaultAddress.zipCode} {defaultAddress.city}
                  {defaultAddress.state ? `, ${defaultAddress.state}` : ""}
                </p>
                <p>{defaultAddress.country}</p>
              </div>
            ) : (
              <p className="pl-7 text-sm text-muted-foreground">
                You don&apos;t have a default address yet
              </p>
            )}
          </Label>

          <Label
            htmlFor="address-mode-different"
            className={cn(
              "flex items-center gap-3 rounded-md border bg-card p-4 font-normal transition-colors",
              mode === "different"
                ? "border-primary"
                : "hover:border-foreground/30",
            )}
          >
            <RadioGroupItem value="different" id="address-mode-different" />
            <span className="text-sm font-medium text-foreground">
              Use a different address
            </span>
          </Label>
        </RadioGroup>

        {mode === "different" && (
          <div className="flex flex-col gap-4 rounded-md border bg-card p-4">
            <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
              <div className="flex flex-col gap-2">
                <Label htmlFor="country">Country</Label>
                <Input
                  id="country"
                  placeholder="Germany"
                  {...register("country")}
                />
                {errors.country && (
                  <p className="text-sm text-destructive">
                    {errors.country.message}
                  </p>
                )}
              </div>
              <div className="flex flex-col gap-2">
                <Label htmlFor="state">State</Label>
                <Input
                  id="state"
                  placeholder="Bavaria"
                  {...register("state")}
                />
                {errors.state && (
                  <p className="text-sm text-destructive">
                    {errors.state.message}
                  </p>
                )}
              </div>
            </div>

            <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
              <div className="flex flex-col gap-2">
                <Label htmlFor="city">City</Label>
                <Input
                  id="city"
                  placeholder="Nuremberg"
                  {...register("city")}
                />
                {errors.city && (
                  <p className="text-sm text-destructive">
                    {errors.city.message}
                  </p>
                )}
              </div>
              <div className="flex flex-col gap-2">
                <Label htmlFor="zipCode">ZIP Code</Label>
                <Input
                  id="zipCode"
                  placeholder="90402"
                  {...register("zipCode")}
                />
                {errors.zipCode && (
                  <p className="text-sm text-destructive">
                    {errors.zipCode.message}
                  </p>
                )}
              </div>
            </div>

            <div className="grid grid-cols-1 gap-4 sm:grid-cols-[1fr_120px]">
              <div className="flex flex-col gap-2">
                <Label htmlFor="street">Street</Label>
                <Input
                  id="street"
                  placeholder="Konigstrasse"
                  {...register("street")}
                />
                {errors.street && (
                  <p className="text-sm text-destructive">
                    {errors.street.message}
                  </p>
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

            <div className="flex items-center gap-2">
              <Checkbox
                id="setAsDefault"
                checked={setAsDefault}
                onCheckedChange={(checked) => setSetAsDefault(checked === true)}
              />
              <Label htmlFor="setAsDefault" className="font-normal">
                Set as default shipping address
              </Label>
            </div>
          </div>
        )}

        {saveError && <p className="text-sm text-destructive">{saveError}</p>}
        {isSaving && (
          <p className="text-sm text-muted-foreground">Saving address…</p>
        )}
      </div>

      <div className="flex flex-col gap-4">
        <h3 className="text-sm font-semibold uppercase tracking-wide">
          Shipping Method
        </h3>

        <div className="flex flex-col gap-3">
          <div className="flex items-center justify-between rounded-md border bg-card p-4">
            <div className="flex flex-col">
              <span className="text-sm font-medium">Standard Delivery</span>
              <span className="text-xs text-muted-foreground">
                Delivery within 3-5 business days
              </span>
            </div>

            <span className="text-sm font-semibold">Free</span>
          </div>
        </div>
      </div>
    </form>
  );
}
