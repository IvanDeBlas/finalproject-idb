import { FC } from "react"
import { Check } from "lucide-react"
import { cn } from "@/lib/utils"

interface WizardStepperProps {
    currentStep: 1 | 2 | 3
    stepLabels?: string[]
}

const DEFAULT_LABELS = ["Seleccionar", "Personalizar", "Confirmar"]

export const WizardStepper: FC<WizardStepperProps> = ({
    currentStep,
    stepLabels = DEFAULT_LABELS,
}) => {
    return (
        <nav aria-label="Progreso del wizard" className="max-w-4xl mx-auto mb-8">
            <div className="flex items-center justify-between">
                {[1, 2, 3].map((step, idx) => {
                    const isCompleted = step < currentStep
                    const isActive = step === currentStep
                    const isFuture = step > currentStep

                    return (
                        <div key={step} className="flex items-center flex-1 last:flex-none">
                            <div className="flex flex-col items-center">
                                <div
                                    className={cn(
                                        "w-10 h-10 rounded-full flex items-center justify-center transition-all duration-300",
                                        isCompleted && "bg-gradient-to-r from-pink-500 to-purple-600",
                                        isActive && "border-2 border-[#a855f7] bg-[#1e2a42]",
                                        isFuture && "border-2 border-[#334155] bg-transparent"
                                    )}
                                    aria-current={isActive ? "step" : undefined}
                                >
                                    {isCompleted ? (
                                        <Check className="w-5 h-5 text-white" />
                                    ) : (
                                        <span
                                            className={cn(
                                                "text-sm font-bold",
                                                isActive && "text-white",
                                                isFuture && "text-[#64748b]"
                                            )}
                                        >
                                            {step}
                                        </span>
                                    )}
                                </div>
                                <span
                                    className={cn(
                                        "mt-2 text-sm font-medium",
                                        isCompleted && "text-[#a855f7]",
                                        isActive && "text-white",
                                        isFuture && "text-[#64748b]"
                                    )}
                                >
                                    {stepLabels[idx]}
                                </span>
                            </div>

                            {idx < 2 && (
                                <div
                                    className={cn(
                                        "flex-1 h-0.5 mx-4",
                                        step < currentStep
                                            ? "bg-gradient-to-r from-pink-500 to-purple-600"
                                            : "border-t-2 border-dashed border-[#334155]"
                                    )}
                                />
                            )}
                        </div>
                    )
                })}
            </div>
        </nav>
    )
}
