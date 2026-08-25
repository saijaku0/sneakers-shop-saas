import { SessionState, Tokens } from "@/shared/api";
import { sessionTerminated, tokenRefreshed } from "@/shared/api/auth-actions";
import { createSlice, type PayloadAction } from "@reduxjs/toolkit";

const initialState = {
  token:
    typeof window !== "undefined" ? localStorage.getItem("accessToken") : null,
  refreshToken:
    typeof window !== "undefined" ? localStorage.getItem("refreshToken") : null,
};

const sessionSlice = createSlice({
  name: "session",
  initialState,
  reducers: {
    setToken: (state, { payload }: PayloadAction<Tokens>) => {
      state.token = payload.accessToken;
      localStorage.setItem("accessToken", payload.accessToken);
      localStorage.setItem("refreshToken", payload.refreshToken);
    },
    clearToken: (state) => {
      state.token = null;
      localStorage.removeItem("accessToken");
      localStorage.removeItem("refreshToken");
    },
  },
  extraReducers: (builder) => {
    builder.addCase(tokenRefreshed, (state, { payload }) => {
      state.token = payload.accessToken;
      state.refreshToken = payload.refreshToken;
      localStorage.setItem("accessToken", payload.accessToken);
      localStorage.setItem("refreshToken", payload.refreshToken);
    });

    builder.addCase(sessionTerminated, (state) => {
      state.token = null;
      state.refreshToken = null;
      localStorage.removeItem("accessToken");
      localStorage.removeItem("refreshToken");
    });
  },
});

export const { setToken, clearToken } = sessionSlice.actions;
export const sessionReducer = sessionSlice.reducer;
export const selectToken = (state: { session: typeof initialState }) =>
  state.session.token;
