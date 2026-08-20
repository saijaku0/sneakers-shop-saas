"use client";

import { useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Input, Label, Button } from "@/shared/ui";
import { Eye, EyeOff } from "lucide-react";
import { updateData, nextStep } from "../model/slice";
import { selectFormData } from "../model/selectors";
import { accountSchema, AccountValues } from "../model/account-schema";

export function AccountStep() {
  const dispatch = useDispatch();
  const data = useSelector(selectFormData);
  const [showPassword, setShowPassword] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<AccountValues>({
    resolver: zodResolver(accountSchema),
    mode: "onBlur",
    defaultValues: {
      name: data.name,
      lastname: data.lastname,
      phoneNumber: data.phoneNumber,
      email: data.email,
      password: data.password,
    },
  });

  const onValid = (values: AccountValues) => {
    dispatch(updateData(values));
    dispatch(nextStep());
  };

  return (
    <form
      id="account-form"
      onSubmit={handleSubmit(onValid)}
      className="flex flex-col gap-6"
    >
      <div className="grid grid-cols-2 gap-4">
        <div className="flex flex-col gap-2">
          <Label htmlFor="name">First Name</Label>
          <Input id="name" placeholder="John" {...register("name")} />
          {errors.name && (
            <p className="text-sm text-destructive">{errors.name.message}</p>
          )}
        </div>
        <div className="flex flex-col gap-2">
          <Label htmlFor="lastname">Last Name</Label>
          <Input id="lastname" placeholder="Doe" {...register("lastname")} />
          {errors.lastname && (
            <p className="text-sm text-destructive">
              {errors.lastname.message}
            </p>
          )}
        </div>
      </div>

      <div className="flex flex-col gap-2">
        <Label htmlFor="phoneNumber">Phone Number</Label>
        <Input
          id="phoneNumber"
          type="tel"
          placeholder="+1 (555) 000-0000"
          {...register("phoneNumber")}
        />
        {errors.phoneNumber && (
          <p className="text-sm text-destructive">
            {errors.phoneNumber.message}
          </p>
        )}
      </div>

      <div className="flex flex-col gap-2">
        <Label htmlFor="email">Email</Label>
        <Input
          id="email"
          type="email"
          placeholder="john.doe@example.com"
          {...register("email")}
        />
        {errors.email && (
          <p className="text-sm text-destructive">{errors.email.message}</p>
        )}
      </div>

      <div className="flex flex-col gap-2">
        <Label htmlFor="password">Password</Label>
        <div className="relative">
          <Input
            id="password"
            type={showPassword ? "text" : "password"}
            placeholder="••••••••"
            className="pr-10"
            {...register("password")}
          />
          <Button
            type="button"
            variant="ghost"
            size="icon"
            onClick={() => setShowPassword((v) => !v)}
            aria-label={showPassword ? "Hide password" : "Show password"}
            className="absolute right-1 h-8 w-8 text-muted-foreground hover:text-foreground"
          >
            {showPassword ? (
              <EyeOff className="h-4 w-4" />
            ) : (
              <Eye className="h-4 w-4" />
            )}
          </Button>
        </div>
        {errors.password && (
          <p className="text-sm text-destructive">{errors.password.message}</p>
        )}
      </div>
    </form>
  );
}
