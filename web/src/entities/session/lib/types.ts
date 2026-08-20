export interface SessionState {
  token: string | null;
}

export interface Tokens {
  accessToken: string;
  refreshToken: string;
}
