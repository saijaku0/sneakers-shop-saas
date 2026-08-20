import type { RootState } from "@/app/store";

export const selectStep = (state: RootState) => state.wizardRegistration.step;
export const selectFormData = (state: RootState) =>
  state.wizardRegistration.formData;
export const selectCard = (state: RootState) => state.wizardRegistration.card;
