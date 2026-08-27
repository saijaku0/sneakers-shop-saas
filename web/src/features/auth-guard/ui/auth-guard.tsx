"use client";

import { useSelector } from "react-redux";
import { selectIsSessionHydrated, selectToken } from "@/entities/session";
import { SignInRequired } from "./sign-in-required";

interface AuthGuardProps {
  children: React.ReactNode;
}

export function AuthGuard({ children }: AuthGuardProps) {
  const isHydrated = useSelector(selectIsSessionHydrated);
  const token = useSelector(selectToken);

  if (!isHydrated) {
    return null;
  }

  if (!token) {
    return <SignInRequired />;
  }

  return <>{children}</>;
}
