import { RegistrationWizard } from "@/features/registration-wizard";
import { Container } from "@/shared/ui";

export default function SignUpPage() {
  return (
    <div className="flex min-h-screen items-center justify-center">
      <Container>
        <RegistrationWizard />
      </Container>
    </div>
  );
}
