import { createApi } from "@reduxjs/toolkit/query/react";
import { baseQueryWithReauth } from "./base-query-with-reauth";

export const api = createApi({
  reducerPath: "sneakersApi",
  baseQuery: baseQueryWithReauth,
  tagTypes: ["Viewer", "Products", "Filters", "Cart"],
  endpoints: () => ({}),
});
