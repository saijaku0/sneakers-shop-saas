import { SidebarInset, SidebarProvider, SidebarTrigger } from "@/shared/ui";
import { ProfileSidebar } from "@/widgets/profile-sidebar";
import { AuthGuard } from "@/features/auth-guard";

export default function ProfileLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <AuthGuard>
      <SidebarProvider>
        <ProfileSidebar />
        <SidebarTrigger />
        <SidebarInset>
          <div className="pr-4 sm:px-6 xl:px-49">{children}</div>
        </SidebarInset>
      </SidebarProvider>
    </AuthGuard>
  );
}
