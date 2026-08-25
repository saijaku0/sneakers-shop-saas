import { api } from "@/shared/api";
import { LoginRequest } from "../lib/types";
import { setToken, Tokens } from "@/entities/session";
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
  }),
  overrideExisting: false,
});

export const { useLoginMutation } = authApi;
