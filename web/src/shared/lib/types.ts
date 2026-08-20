export interface DefaultShippingAddress {
  country: string;
  state?: string;
  city: string;
  street: string;
  houseNumber: string;
  zipCode: string;
}

export interface CardData {
  cardNumber: string;
  holder: string;
  expiry: string;
  cvv: string;
}
