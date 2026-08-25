export interface Tokens {
  accessToken: string;
  refreshToken: string;
}

export interface SessionState {
  session: { accessToken: string | null; refreshToken: string | null };
}
