import { api } from "@/shared/api";
import { BrandDto } from "../model/types";

export const brandApi = api.injectEndpoints({
  endpoints: (builder) => ({
    getBrands: builder.query<BrandDto[], void>({
      query: () => "/products/brands",
      providesTags: ["Brand"],
    }),
  }),
});

export const { useGetBrandsQuery } = brandApi;
