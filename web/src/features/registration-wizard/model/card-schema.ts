import { z } from "zod";

const luhnValid = (value: string) => {
  const digits = value.replace(/\s/g, "");
  if (!/^\d{13,19}$/.test(digits)) return false;
  let sum = 0;
  let double = false;
  for (let i = digits.length - 1; i >= 0; i--) {
    let d = Number(digits[i]);
    if (double) {
      d *= 2;
      if (d > 9) d -= 9;
    }
    sum += d;
    double = !double;
  }
  return sum % 10 === 0;
};

const expiryValid = (value: string) => {
  const m = /^(\d{2})\/(\d{2})$/.exec(value.trim());
  if (!m) return false;
  const month = Number(m[1]);
  const year = 2000 + Number(m[2]);
  if (month < 1 || month > 12) return false;
  const now = new Date();
  const end = new Date(year, month, 0, 23, 59, 59);
  return end >= now;
};

export const cardSchema = z.object({
  cardNumber: z
    .string()
    .min(1, "Card number is required")
    .refine(luhnValid, "Enter a valid card number"),
  holder: z.string().min(1, "Cardholder name is required"),
  expiry: z
    .string()
    .min(1, "Expiration date is required")
    .refine(expiryValid, "Enter a valid date (MM/YY)"),
  cvv: z.string().regex(/^\d{3,4}$/, "3 or 4 digits"),
});
