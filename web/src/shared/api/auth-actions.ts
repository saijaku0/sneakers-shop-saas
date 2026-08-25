import { createAction } from "@reduxjs/toolkit";
import { Tokens } from "./model/types";

export const tokenRefreshed = createAction<Tokens>("api/tokenRefreshed");
export const sessionTerminated = createAction("api/sessionTerminated");
