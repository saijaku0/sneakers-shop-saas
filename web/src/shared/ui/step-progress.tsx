import { cn } from "../lib";

interface StepProgressProps {
  currentStep: number;
  totalSteps?: number;
  labels?: string[];
}

export function StepProgress({
  currentStep,
  totalSteps = 3,
  labels,
}: StepProgressProps) {
  const stepsCount = labels ? labels.length : totalSteps;
  return (
    <div className="flex gap-2 w-full">
      {Array.from({ length: stepsCount }, (_, i) => {
        const stepNumber = i + 1;
        const isCompleted = stepNumber < currentStep;
        const isCurrent = stepNumber === currentStep;
        const isActive = stepNumber <= currentStep;

        return (
          <div key={stepNumber} className="flex flex-1 flex-col gap-2">
            <div
              className={cn(
                "h-1 w-full rounded-full transition-colors duration-300",
                isActive ? "bg-primary" : "bg-border",
              )}
            />

            {labels && labels[i] && (
              <span
                className={cn(
                  "text-xs font-semibold uppercase tracking-wider transition-colors",
                  isCurrent
                    ? "text-foreground"
                    : isCompleted
                      ? "text-primary/70"
                      : "text-muted-foreground",
                )}
              >
                {labels[i]}
              </span>
            )}
          </div>
        );
      })}
    </div>
  );
}
