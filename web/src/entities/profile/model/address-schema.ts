import { z } from "zod";

export const addressSchema = z
  .object({
    country: z.string(),
    state: z.string().nullable().optional(),
    city: z.string(),
    street: z.string(),
    houseNumber: z.string(),
    zipCode: z.string(),
  })
  .superRefine((val, ctx) => {
    const required = [
      "country",
      "city",
      "street",
      "houseNumber",
      "zipCode",
    ] as const;
    const filled = required.filter((k) => val[k]?.trim());

    if (filled.length === 0) return;

    for (const k of required) {
      if (!val[k]?.trim()) {
        ctx.addIssue({
          code: z.ZodIssueCode.custom,
          path: [k],
          message: "Complete the address or clear all fields",
        });
      }
    }
  });
