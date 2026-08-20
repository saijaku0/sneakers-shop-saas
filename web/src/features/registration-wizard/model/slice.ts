import { createSlice } from "@reduxjs/toolkit";
import { RegistrationRequest } from "../lib/types";
import { CardData } from "@/shared/lib";

type RegisterState = {
  step: number;
  formData: RegistrationRequest;
  card: CardData | null;
};

const initialState: RegisterState = {
  step: 1,
  formData: {
    name: "",
    lastname: "",
    phoneNumber: "",
    email: "",
    password: "",
  },
  card: null,
};

const wizardRegistrationSlice = createSlice({
  name: "wizard-registration",
  initialState,
  reducers: {
    nextStep: (state) => {
      state.step += 1;
    },
    prevStep: (state) => {
      state.step -= 1;
    },
    updateData: (state, { payload }) => {
      state.formData = { ...state.formData, ...payload };
    },
    setCard: (state, { payload }) => {
      state.card = payload;
    },
    resetWizard: () => initialState,
  },
});

export const { nextStep, prevStep, updateData, setCard, resetWizard } =
  wizardRegistrationSlice.actions;
export const wizardRegistrationReducer = wizardRegistrationSlice.reducer;
