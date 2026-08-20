import { z } from "zod";

export const accountSchema = z.object({
  name: z.string().min(1, "First name is required"),
  lastname: z.string().min(1, "Last name is required"),
  phoneNumber: z
    .string()
    .min(1, "Phone number is required")
    .regex(/^[+]?[\d\s()-]{7,}$/, "Enter a valid phone number"),
  email: z.string().min(1, "Email is required").email("Enter a valid email"),
  password: z
    .string()
    .min(8, "At least 8 characters")
    .regex(/[A-Z]/, "One uppercase letter")
    .regex(/[a-z]/, "One lowercase letter")
    .regex(/\d/, "One number")
    .regex(/[^A-Za-z0-9]/, "One special character"),
});

export type AccountValues = z.infer<typeof accountSchema>;
