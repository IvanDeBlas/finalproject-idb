"use client"

import { Check } from "lucide-react"

const PASOS_PROGRAMA = [
    { numero: 1, label: "Datos basicos" },
    { numero: 2, label: "Comisiones" },
    { numero: 3, label: "Definir tareas" },
    { numero: 4, label: "Revisar" },
]

interface PromoProgramaWizardStepperProps {
    pasoActual: number
    pasosCompletados: number[]
    onPasoClick?: (paso: number) => void
}

export function PromoProgramaWizardStepper({
    pasoActual,
    pasosCompletados,
    onPasoClick,
}: PromoProgramaWizardStepperProps) {
    return (
        <div className="flex items-center justify-center gap-0 py-4" role="progressbar" aria-label="Progreso del wizard">
            {PASOS_PROGRAMA.map((paso, index) => {
                const isCompleted = pasosCompletados.includes(paso.numero)
                const isActive = paso.numero === pasoActual
                const isClickable = isCompleted && !isActive

                return (
                    <div key={paso.numero} className="flex items-center">
                        <button
                            type="button"
                            className={`flex flex-col items-center gap-1 ${isClickable ? "cursor-pointer" : "cursor-default"}`}
                            onClick={() => isClickable && onPasoClick?.(paso.numero)}
                            disabled={!isClickable}
                            aria-current={isActive ? "step" : undefined}
                        >
                            <div
                                className={`flex h-8 w-8 items-center justify-center rounded-full text-sm font-medium transition-colors ${
                                    isActive
                                        ? "bg-gradient-to-r from-pink-500 to-purple-600 text-white"
                                        : isCompleted
                                            ? "bg-emerald-500 text-white"
                                            : "bg-zinc-700 text-zinc-400"
                                }`}
                            >
                                {isCompleted && !isActive ? (
                                    <Check className="h-4 w-4" />
                                ) : (
                                    paso.numero
                                )}
                            </div>
                            <span
                                className={`text-xs whitespace-nowrap ${
                                    isActive
                                        ? "text-white font-medium"
                                        : isCompleted
                                            ? "text-emerald-400"
                                            : "text-zinc-500"
                                }`}
                            >
                                {paso.label}
                            </span>
                        </button>

                        {index < PASOS_PROGRAMA.length - 1 && (
                            <div
                                className={`h-[2px] w-12 sm:w-16 mx-2 transition-colors ${
                                    isCompleted ? "bg-emerald-500" : "bg-zinc-700"
                                }`}
                            />
                        )}
                    </div>
                )
            })}
        </div>
    )
}
