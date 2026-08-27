import type { DefaultShippingAddress } from "@/shared/lib";

export type Address = DefaultShippingAddress;

export interface AddressResponse {
  address: Address | null;
}

export interface UpdateAddressRequest {
  address: Address;
}
