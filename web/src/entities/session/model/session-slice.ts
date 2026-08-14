import { createSlice, type PayloadAction } from "@reduxjs/toolkit";
import { SessionState } from "../lib/types";

const initialState: SessionState = {
  token:
    typeof window !== "undefined" ? localStorage.getItem("accessToken") : null,
};

const sessionSlice = createSlice({
  name: "session",
  initialState,
  reducers: {
    setToken: (state, { payload }: PayloadAction<string>) => {
      state.token = payload;
      localStorage.setItem("accessToken", payload);
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
