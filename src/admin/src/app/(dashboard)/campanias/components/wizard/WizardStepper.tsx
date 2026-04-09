"use client"

import { Fragment } from "react"
import { Check } from "lucide-react"
import { cn } from "@/lib/utils"

interface WizardStepperProps {
    currentStep: number
    totalSteps: number
    completedSteps: number[]
    onStepClick?: (step: number) => void
}

const STEPS = [
    { number: 1, label: "Template" },
    { number: 2, label: "Informacion Basica" },
    { number: 3, label: "Historia" },
    { number: 4, label: "Recompensas" },
    { number: 5, label: "Revision" },
]

export function WizardStepper({
    currentStep,
    totalSteps,
    completedSteps,
    onStepClick,
}: WizardStepperProps) {
    return (
        <>
            {/* Desktop stepper */}
            <div
                className="hidden md:flex items-center justify-between max-w-2xl mx-auto mb-10"
                role="progressbar"
                aria-valuenow={currentStep}
                aria-valuemin={1}
                aria-valuemax={totalSteps}
                aria-label="Progreso de creacion de campania"
            >
                {STEPS.map((step, index) => {
                    const isActive = step.number === currentStep
                    const isCompleted = completedSteps.includes(step.number)
                    const isClickable = isCompleted && !!onStepClick

                    return (
                        <Fragment key={step.number}>
                            <div className="flex flex-col items-center">
                                <button
                                    type="button"
                                    onClick={() =>
                                        isClickable && onStepClick(step.number)
                                    }
                                    disabled={!isClickable}
                                    className={cn(
                                        "w-12 h-12 rounded-full flex items-center justify-center font-bold text-sm transition-all duration-200",
                                        isActive &&
                                            "bg-gradient-to-r from-pink-500 to-purple-600 text-white shadow-lg shadow-purple-500/25",
                                        isCompleted &&
                                            !isActive &&
                                            "bg-gradient-to-r from-pink-500 to-purple-600 text-white",
                                        !isActive &&
                                            !isCompleted &&
                                            "bg-muted/20 text-muted-foreground border border-border",
                                        isClickable &&
                                            "cursor-pointer hover:scale-105"
                                    )}
                                    aria-current={
                                        isActive ? "step" : undefined
                                    }
                                >
                                    {isCompleted && !isActive ? (
                                        <Check className="w-5 h-5" />
                                    ) : (
                                        step.number
                                    )}
                                </button>
                                <span
                                    className={cn(
                                        "text-sm mt-2 text-center transition-colors",
                                        isActive
                                            ? "text-foreground font-medium"
                                            : "text-muted-foreground"
                                    )}
                                >
                                    {step.label}
                                </span>
                            </div>

                            {index < STEPS.length - 1 && (
                                <div className="flex-1 h-0.5 mx-2 bg-muted/20 relative overflow-hidden">
                                    <div
                                        className={cn(
                                            "h-full bg-gradient-to-r from-pink-500 to-purple-600 transition-all duration-500 ease-in-out",
                                            isCompleted
                                                ? "w-full"
                                                : "w-0"
                                        )}
                                    />
                                </div>
                            )}
                        </Fragment>
                    )
                })}
            </div>

            {/* Mobile stepper */}
            <div className="md:hidden text-center mb-8">
                <p className="text-sm text-muted-foreground">
                    Paso {currentStep} de {totalSteps}
                </p>
                <p className="text-xs text-muted-foreground mt-1">
                    {STEPS[currentStep - 1]?.label}
                </p>
                <div className="flex gap-1 justify-center mt-3">
                    {STEPS.map((step) => (
                        <div
                            key={step.number}
                            className={cn(
                                "h-1.5 rounded-full transition-all duration-300",
                                step.number === currentStep
                                    ? "w-8 bg-gradient-to-r from-pink-500 to-purple-600"
                                    : completedSteps.includes(step.number)
                                      ? "w-4 bg-purple-500"
                                      : "w-4 bg-muted/30"
                            )}
                        />
                    ))}
                </div>
            </div>
        </>
    )
}
