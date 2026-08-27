export type {
  Address,
  AddressResponse,
  UpdateAddressRequest,
} from "./lib/types";
export { addressSchema } from "./model/address-schema";
export {
  useGetAddressQuery,
  useUpdateAddressMutation,
} from "./api/profile-api";
