import { api } from "@/shared/api";
import { sessionReducer } from "@/entities/session";
import { configureStore } from "@reduxjs/toolkit";
import { wizardRegistrationReducer } from "@/features/registration-wizard";
import { cartReducer } from "@/entities/cart";

export const store = configureStore({
  reducer: {
    [api.reducerPath]: api.reducer,
    session: sessionReducer,
    wizardRegistration: wizardRegistrationReducer,
    cart: cartReducer,
  },
  middleware: (getDefault) => getDefault().concat(api.middleware),
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
