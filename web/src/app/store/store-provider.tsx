"use client";
import { Provider } from "react-redux";
import { store } from "./store";
import { SessionHydrator } from "@/entities/session";

export function StoreProvider({ children }: { children: React.ReactNode }) {
  return (
    <Provider store={store}>
      <SessionHydrator />
      {children}
    </Provider>
  );
}
