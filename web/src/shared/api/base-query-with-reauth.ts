import {
  BaseQueryFn,
  FetchArgs,
  fetchBaseQuery,
  FetchBaseQueryError,
} from "@reduxjs/toolkit/query";
import { Mutex } from "async-mutex";
import { sessionTerminated, tokenRefreshed } from "./auth-actions";
import { SessionState, Tokens } from "./model/types";

const mutex = new Mutex();

const baseQuery = fetchBaseQuery({
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
});

export const baseQueryWithReauth: BaseQueryFn<
  string | FetchArgs,
  unknown,
  FetchBaseQueryError
> = async (args, api, extraOptions) => {
  await mutex.waitForUnlock();

  const result = await baseQuery(args, api, extraOptions);

  if (result.error?.status !== 401) {
    return result;
  }

  if (mutex.isLocked()) {
    await mutex.waitForUnlock();

    return baseQuery(args, api, extraOptions);
  }

  const release = await mutex.acquire();

  try {
    const { refreshToken } = (api.getState() as SessionState).session;

    if (!refreshToken) {
      api.dispatch(sessionTerminated());
      return result;
    }

    const refreshResult = await baseQuery(
      {
        url: "/auth/refresh",
        method: "POST",
        body: { refreshToken },
      },
      api,
      extraOptions,
    );

    if (refreshResult.error || !refreshResult.data) {
      api.dispatch(sessionTerminated());
      return result;
    }

    api.dispatch(tokenRefreshed(refreshResult.data as Tokens));

    return baseQuery(args, api, extraOptions);
  } finally {
    release();
  }
};
