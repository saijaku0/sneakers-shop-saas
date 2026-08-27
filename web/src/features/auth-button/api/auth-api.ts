import { api, Tokens } from "@/shared/api";
import { LoginRequest } from "../lib/types";
import { setToken } from "@/entities/session";
import { startCartSync } from "@/entities/cart";

export const authApi = api.injectEndpoints({
  endpoints: (builder) => ({
    login: builder.mutation<Tokens, LoginRequest>({
      query: (credentials) => ({
        url: "/auth/login",
        method: "POST",
        body: credentials,
      }),
      async onQueryStarted(_, { dispatch, queryFulfilled }) {
        const { data } = await queryFulfilled;
        dispatch(setToken(data));
        dispatch(startCartSync());
      },
      invalidatesTags: ["Viewer"],
    }),
    logout: builder.mutation<void, { refreshToken: string }>({
      query: (body) => ({
        url: "/auth/logout",
        method: "POST",
        body,
      }),
    }),
  }),
  overrideExisting: false,
});

export const { useLoginMutation, useLogoutMutation } = authApi;
