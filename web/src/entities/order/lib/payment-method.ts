import { PaymentMethod, type PaymentMethodName } from "./types";

export const PAYMENT_METHOD_LABELS: Record<PaymentMethod, string> = {
  [PaymentMethod.CreditCard]: "Credit Card",
  [PaymentMethod.DebitCard]: "Debit Card",
  [PaymentMethod.PayPal]: "PayPal",
  [PaymentMethod.Cash]: "Cash",
};

const PAYMENT_METHOD_NAME_TO_ENUM: Record<PaymentMethodName, PaymentMethod> = {
  CreditCard: PaymentMethod.CreditCard,
  DebitCard: PaymentMethod.DebitCard,
  PayPal: PaymentMethod.PayPal,
  Cash: PaymentMethod.Cash,
};

export function formatPaymentMethodName(name: PaymentMethodName): string {
  return PAYMENT_METHOD_LABELS[PAYMENT_METHOD_NAME_TO_ENUM[name]] ?? name;
}
