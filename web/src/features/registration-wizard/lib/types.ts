import { DefaultShippingAddress } from "@/shared/lib";

export interface RegistrationRequest {
  name: string;
  lastname: string;
  phoneNumber: string;
  email: string;
  password: string;
  defaultShippingAddress?: DefaultShippingAddress;
}
