"use client";

import Link from "next/link";
import { useDispatch, useSelector } from "react-redux";
import {
  Avatar,
  AvatarFallback,
  Button,
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/shared/ui";
import {
  selectToken,
  clearToken,
  selectRefreshToken,
} from "@/entities/session";
import { cn } from "@/shared/lib";
import { User } from "lucide-react";
import { LoginForm } from "./login-form";
import { useState } from "react";
import { clearCart } from "@/entities/cart";
import { useLogoutMutation } from "../api/auth-api";
import { api } from "@/shared/api";

export function AuthButton({
  orientation = "row",
  size,
}: {
  orientation?: "row" | "col";
  size?: React.ComponentProps<typeof Button>["size"];
}) {
  const [signInOpen, setSignInOpen] = useState(false);
  const token = useSelector(selectToken);
  const refreshToken = useSelector(selectRefreshToken);
  const dispatch = useDispatch();

  const [logoutApi] = useLogoutMutation();

  const handleLogout = async () => {
    try {
      if (refreshToken) {
        await logoutApi({ refreshToken }).unwrap();
      }
    } catch (error) {
      console.error("Backend logout failed", error);
    } finally {
      dispatch(clearToken());
      dispatch(clearCart());
      dispatch(api.util.resetApiState());
    }
  };

  if (token) {
    return (
      <DropdownMenu modal={false}>
        <DropdownMenuTrigger asChild>
          <button className="rounded-full outline-none focus-visible:ring-2 focus-visible:ring-ring">
            <Avatar className="size-9 cursor-pointer">
              <AvatarFallback className="bg-primary text-primary-foreground text-sm font-bold">
                <User className="size-6" />
              </AvatarFallback>
            </Avatar>
          </button>
        </DropdownMenuTrigger>
        <DropdownMenuContent align="end" className="w-48">
          <DropdownMenuItem asChild>
            <Link href="/profile/personal">Profile</Link>
          </DropdownMenuItem>
          <DropdownMenuItem asChild>
            <Link href="/profile/orders">Orders</Link>
          </DropdownMenuItem>
          <DropdownMenuSeparator />
          <DropdownMenuItem
            onClick={handleLogout}
            className="text-destructive focus:text-destructive"
          >
            Sign out
          </DropdownMenuItem>
        </DropdownMenuContent>
      </DropdownMenu>
    );
  }

  return (
    <div
      className={cn(
        "flex gap-2",
        orientation === "col" ? "flex-col w-full" : "items-center",
      )}
    >
      <Dialog open={signInOpen} onOpenChange={setSignInOpen}>
        <DialogTrigger asChild>
          <Button variant="ghost" size={size}>
            Sign in
          </Button>
        </DialogTrigger>
        <DialogContent className="sm:max-w-md">
          <DialogHeader>
            <DialogTitle>Sign in</DialogTitle>
          </DialogHeader>
          <LoginForm onSuccess={() => setSignInOpen(false)} />
        </DialogContent>
      </Dialog>

      <Button asChild size={size}>
        <Link href="/sign-up">Sign up</Link>
      </Button>
    </div>
  );
}
