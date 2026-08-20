import { setToken, Tokens } from "@/entities/session";
import { api } from "@/shared/api";
import { RegistrationRequest } from "../lib/types";

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
      },
      invalidatesTags: ["Viewer"],
    }),
  }),
  overrideExisting: false,
});

export const { useRegisterMutation } = registerApi;
