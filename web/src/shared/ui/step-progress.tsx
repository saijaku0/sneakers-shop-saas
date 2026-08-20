interface StepProgressProps {
  currentStep: number;
  totalSteps?: number;
}

export function StepProgress({
  currentStep,
  totalSteps = 3,
}: StepProgressProps) {
  return (
    <div className="flex gap-2">
      {Array.from({ length: totalSteps }, (_, i) => {
        const stepNumber = i + 1;
        const isActive = stepNumber <= currentStep;
        return (
          <div
            key={stepNumber}
            className={`h-1 flex-1 rounded-full transition-colors duration-300 ${
              isActive ? "bg-primary" : "bg-border"
            }`}
          />
        );
      })}
    </div>
  );
}
