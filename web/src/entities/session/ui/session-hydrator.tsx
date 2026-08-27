"use client";

import { useEffect } from "react";
import { useDispatch } from "react-redux";
import { hydrateSession } from "../model/session-slice";

export function SessionHydrator() {
  const dispatch = useDispatch();

  useEffect(() => {
    dispatch(hydrateSession());
  }, [dispatch]);

  return null;
}
