import { createSlice, type PayloadAction } from "@reduxjs/toolkit";
import { SessionState, Tokens } from "../lib/types";

const initialState: SessionState = {
  token:
    typeof window !== "undefined" ? localStorage.getItem("accessToken") : null,
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
});

export const { setToken, clearToken } = sessionSlice.actions;
export const sessionReducer = sessionSlice.reducer;
export const selectToken = (state: { session: SessionState }) =>
  state.session.token;
