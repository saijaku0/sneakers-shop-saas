import { SessionState, Tokens } from "@/shared/api";
import { sessionTerminated, tokenRefreshed } from "@/shared/api/auth-actions";
import { createSlice, type PayloadAction } from "@reduxjs/toolkit";

interface SessionSliceState extends SessionState {
  isHydrated: boolean;
}

const initialState: SessionSliceState = {
  session: {
    accessToken: null,
    refreshToken: null,
  },
  isHydrated: false,
};

const sessionSlice = createSlice({
  name: "session",
  initialState,
  reducers: {
    hydrateSession: (state) => {
      state.session.accessToken = localStorage.getItem("accessToken");
      state.session.refreshToken = localStorage.getItem("refreshToken");
      state.isHydrated = true;
    },
    setToken: (state, { payload }: PayloadAction<Tokens>) => {
      state.session.accessToken = payload.accessToken;
      state.session.refreshToken = payload.refreshToken;
      localStorage.setItem("accessToken", payload.accessToken);
      localStorage.setItem("refreshToken", payload.refreshToken);
    },
    clearToken: (state) => {
      state.session.accessToken = null;
      state.session.refreshToken = null;
      localStorage.removeItem("accessToken");
      localStorage.removeItem("refreshToken");
    },
  },
  extraReducers: (builder) => {
    builder.addCase(tokenRefreshed, (state, { payload }) => {
      state.session.accessToken = payload.accessToken;
      state.session.refreshToken = payload.refreshToken;
      localStorage.setItem("accessToken", payload.accessToken);
      localStorage.setItem("refreshToken", payload.refreshToken);
    });

    builder.addCase(sessionTerminated, (state) => {
      state.session.accessToken = null;
      state.session.refreshToken = null;
      localStorage.removeItem("accessToken");
      localStorage.removeItem("refreshToken");
    });
  },
});

export const { hydrateSession, setToken, clearToken } = sessionSlice.actions;
export const sessionReducer = sessionSlice.reducer;
export const selectToken = (state: { session: SessionSliceState }) =>
  state.session.session.accessToken;
export const selectRefreshToken = (state: { session: SessionSliceState }) =>
  state.session.session.refreshToken;
export const selectIsSessionHydrated = (state: {
  session: SessionSliceState;
}) => state.session.isHydrated;
