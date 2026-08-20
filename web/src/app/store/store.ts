import { api } from "@/shared/api";
import { sessionReducer } from "@/entities/session";
import { configureStore } from "@reduxjs/toolkit";
import { wizardRegistrationReducer } from "@/features/registration-wizard";

export const store = configureStore({
  reducer: {
    [api.reducerPath]: api.reducer,
    session: sessionReducer,
    wizardRegistration: wizardRegistrationReducer,
  },
  middleware: (getDefault) => getDefault().concat(api.middleware),
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
