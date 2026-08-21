import { api } from "@/shared/api";
import {
  CatalogFilters,
  ProductDetail,
  ProductFilters,
  ProductsPageResult,
} from "../model/types";

export const productsApi = api.injectEndpoints({
  endpoints: (builder) => ({
    getProducts: builder.query<ProductsPageResult, CatalogFilters>({
      query: (filters) => ({
        url: "/products",
        params: {
          ...(filters.gender && { gender: filters.gender }),
          categories: filters.categories,
          colors: filters.colors,
          sizes: filters.sizes,
          brands: filters.brands,
          ...(filters.minPrice !== undefined && { minPrice: filters.minPrice }),
          ...(filters.maxPrice !== undefined && { maxPrice: filters.maxPrice }),
          ...(filters.sortBy &&
            filters.sortBy !== "popular" && { sortBy: filters.sortBy }),
          pageNumber: filters.page,
        },
      }),
      providesTags: ["Products"],
    }),
    getProductById: builder.query<ProductDetail, string>({
      query: (id) => ({ url: `/products/${id}` }),
      providesTags: (_result, _err, id) => [{ type: "Products", id }],
    }),
    getFilters: builder.query<ProductFilters, void>({
      query: () => ({ url: "/products/filters" }),
      providesTags: ["Filters"],
    }),
  }),
  overrideExisting: false,
});

export const {
  useGetProductsQuery,
  useGetProductByIdQuery,
  useGetFiltersQuery,
} = productsApi;
