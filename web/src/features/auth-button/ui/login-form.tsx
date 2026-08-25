"use client";

import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { useState } from "react";
import { Input, Label, Button } from "@/shared/ui";
import { Eye, EyeOff } from "lucide-react";
import { useLoginMutation } from "../api/auth-api";
import {
  clearLocalCart,
  finishCartSync,
  selectCartItems,
  useLazyGetCartQuery,
  useSyncCartMutation,
} from "@/entities/cart";
import { useDispatch, useSelector } from "react-redux";

const loginSchema = z.object({
  email: z.string().min(1, "Email is required").email("Enter a valid email"),
  password: z.string().min(1, "Password is required"),
});

type LoginValues = z.infer<typeof loginSchema>;

export function LoginForm({ onSuccess }: { onSuccess?: () => void }) {
  const [login, { isLoading: isLoginLoading }] = useLoginMutation();
  const [syncCart, { isLoading: isSyncingCart }] = useSyncCartMutation();
  const [fetchCart, { isFetching: isFetchingCart }] = useLazyGetCartQuery();
  const cartItems = useSelector(selectCartItems);
  const [showPassword, setShowPassword] = useState(false);
  const [serverError, setServerError] = useState<string | null>(null);
  const dispatch = useDispatch();

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginValues>({
    resolver: zodResolver(loginSchema),
    mode: "onBlur",
    defaultValues: { email: "", password: "" },
  });

  const onValid = async (values: LoginValues) => {
    setServerError(null);

    const guestCartItems = cartItems;

    try {
      await login(values).unwrap();
    } catch (error) {
      console.error("Login error:", error);
      setServerError("Invalid email or password");
      return;
    }

    try {
      if (guestCartItems.length > 0) {
        await syncCart(
          guestCartItems.map((item) => ({
            warehouseItemId: item.warehouseItemId,
            quantity: item.quantity,
          })),
        ).unwrap();
        await fetchCart().unwrap();
        dispatch(clearLocalCart());
      } else {
        await fetchCart().unwrap();
      }
    } catch (error) {
      console.error("Cart sync error:", error);
    } finally {
      dispatch(finishCartSync());
    }

    onSuccess?.();
  };

  const isLoading = isLoginLoading || isSyncingCart || isFetchingCart;

  return (
    <form onSubmit={handleSubmit(onValid)} className="flex flex-col gap-5">
      <div className="flex flex-col gap-2">
        <Label htmlFor="login-email">Email</Label>

        <Input
          id="login-email"
          type="email"
          placeholder="john.doe@example.com"
          {...register("email")}
        />

        {errors.email && (
          <p className="text-sm text-destructive">{errors.email.message}</p>
        )}
      </div>

      <div className="flex flex-col gap-2">
        <Label htmlFor="login-password">Password</Label>

        <div className="relative">
          <Input
            id="login-password"
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

      {serverError && <p className="text-sm text-destructive">{serverError}</p>}

      <Button type="submit" size="lg" className="w-full" disabled={isLoading}>
        {isLoginLoading
          ? "Signing in…"
          : isSyncingCart || isFetchingCart
            ? "Syncing cart…"
            : "Sign in"}
      </Button>
    </form>
  );
}
