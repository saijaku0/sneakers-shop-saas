import { Lock } from "lucide-react";
import { AuthButton } from "@/features/auth-button";

export function SignInRequired() {
  return (
    <div className="flex min-h-[70vh] flex-col items-center justify-center gap-6 px-4 text-center">
      <Lock className="h-10 w-10 text-muted-foreground" />

      <div className="flex flex-col gap-2">
        <h1 className="font-display text-2xl font-bold uppercase tracking-tight text-foreground">
          Authentication required
        </h1>
        <p className="max-w-sm text-sm text-muted-foreground">
          Sign in to your account to continue, or create a new one if you
          don&apos;t have one yet.
        </p>
      </div>

      <AuthButton size="lg" />
    </div>
  );
}
