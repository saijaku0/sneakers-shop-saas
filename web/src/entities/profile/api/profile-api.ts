import { api } from "@/shared/api";
import type { AddressResponse, UpdateAddressRequest } from "../lib/types";

export const profileApi = api.injectEndpoints({
  endpoints: (builder) => ({
    getAddress: builder.query<AddressResponse, void>({
      query: () => ({
        url: "/profile/address",
        method: "GET",
      }),
      providesTags: ["Address"],
    }),
    updateAddress: builder.mutation<void, UpdateAddressRequest>({
      query: (body) => ({
        url: "/profile/address",
        method: "PUT",
        body,
      }),
      invalidatesTags: ["Address"],
    }),
  }),
  overrideExisting: false,
});

export const { useGetAddressQuery, useUpdateAddressMutation } = profileApi;
