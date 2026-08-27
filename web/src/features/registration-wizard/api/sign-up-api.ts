import { setToken } from "@/entities/session";
import { api, Tokens } from "@/shared/api";
import { RegistrationRequest } from "../lib/types";
import { startCartSync } from "@/entities/cart";

export const registerApi = api.injectEndpoints({
  endpoints: (builder) => ({
    register: builder.mutation<Tokens, RegistrationRequest>({
      query: (credentials) => ({
        url: "/auth/register",
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

export const { useRegisterMutation } = registerApi;
