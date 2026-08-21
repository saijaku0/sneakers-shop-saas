import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";

export const api = createApi({
  reducerPath: "sneakersApi",
  baseQuery: fetchBaseQuery({
    baseUrl: process.env.NEXT_PUBLIC_BASE_API_URL,
    paramsSerializer: (params) => {
      const search = new URLSearchParams();
      Object.entries(params).forEach(([key, value]) => {
        if (Array.isArray(value)) {
          value
            .filter((v) => v !== undefined && v !== null && v !== "")
            .forEach((v) => search.append(key, String(v)));
        } else if (value !== undefined && value !== null && value !== "") {
          search.append(key, String(value));
        }
      });
      return search.toString();
    },
    prepareHeaders: (headers) => {
      const token =
        typeof window !== "undefined"
          ? localStorage.getItem("accessToken")
          : null;
      if (token) headers.set("Authorization", `Bearer ${token}`);
      return headers;
    },
  }),
  tagTypes: ["Viewer", "Products", "Filters"],
  endpoints: () => ({}),
});
