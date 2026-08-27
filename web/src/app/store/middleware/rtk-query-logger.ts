import {
  isRejectedWithValue,
  type Middleware,
  type UnknownAction,
} from "@reduxjs/toolkit";
import { toast } from "sonner";

interface ErrorPayload {
  status?: number;
  data?: {
    detail?: string;
    title?: string;
  };
}

interface MutationAction extends UnknownAction {
  meta?: {
    arg?: {
      endpointName?: string;
    };
  };
}

export const rtkQueryErrorLogger: Middleware =
  () => (next) => (action: unknown) => {
    if (isRejectedWithValue(action)) {
      const payload = action.payload as ErrorPayload;

      const errorData = payload.data;
      const errorStatus = payload.status;

      const errorMessage =
        errorData?.detail ||
        errorData?.title ||
        "An unexpected error occurred. Please try again.";

      if (errorStatus !== 401) {
        toast.error(errorMessage);
      }
    }

    const mutationAction = action as MutationAction;

    if (mutationAction.type?.endsWith("executeMutation/fulfilled")) {
      const endpointName = mutationAction.meta?.arg?.endpointName;

      const successMessages: Record<string, string> = {
        registerUser: "Registration successful! Welcome.",
        login: "You have successfully logged in.",
        updateShippingAddress: "Shipping address has been updated.",
        addToCart: "Item successfully added to your cart.",
      };

      if (endpointName && successMessages[endpointName]) {
        toast.success(successMessages[endpointName]);
      }
    }

    return next(action);
  };
