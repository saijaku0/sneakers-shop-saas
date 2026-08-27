"use client";

import { AuthGuard } from "@/features/auth-guard";
import { CheckoutWizard } from "@/features/checkout-wizard";

export default function CheckoutPage() {
  return (
    <AuthGuard>
      <CheckoutWizard />
    </AuthGuard>
  );
}
