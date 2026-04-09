"use client"

import { Button } from "@/components/ui/button"
import { ChevronLeft, Rocket, Save, Loader2 } from "lucide-react"

interface PromoProgramaWizardFooterProps {
    pasoActual: number
    totalPasos?: number
    onAnterior: () => void
    onSiguiente: () => void
    isSubmitting: boolean
    mode: "create" | "edit"
}

export function PromoProgramaWizardFooter({
    pasoActual,
    totalPasos = 4,
    onAnterior,
    onSiguiente,
    isSubmitting,
    mode,
}: PromoProgramaWizardFooterProps) {
    const isLastStep = pasoActual === totalPasos
    const isFormStep = pasoActual <= 2

    const submitLabel = mode === "create" ? "Publicar programa" : "Guardar cambios"
    const SubmitIcon = mode === "create" ? Rocket : Save

    return (
        <div className="flex items-center justify-between h-16 px-4 bg-[#0d0d1a] border-t border-zinc-800">
            <Button
                variant="outline"
                size="sm"
                disabled={pasoActual === 1 || isSubmitting}
                onClick={onAnterior}
            >
                <ChevronLeft className="h-4 w-4 mr-1" />
                Anterior
            </Button>

            <span className="text-xs text-zinc-500">
                Paso {pasoActual} de {totalPasos}
            </span>

            {isLastStep ? (
                <Button
                    size="sm"
                    className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700"
                    onClick={onSiguiente}
                    disabled={isSubmitting}
                >
                    {isSubmitting ? (
                        <Loader2 className="h-4 w-4 mr-2 animate-spin" />
                    ) : (
                        <SubmitIcon className="h-4 w-4 mr-2" />
                    )}
                    {isSubmitting ? "Guardando..." : submitLabel}
                </Button>
            ) : isFormStep ? (
                <Button
                    type="submit"
                    form={`paso-${pasoActual}-form`}
                    size="sm"
                    className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700"
                    disabled={isSubmitting}
                >
                    Siguiente
                </Button>
            ) : (
                <Button
                    size="sm"
                    className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700"
                    onClick={onSiguiente}
                    disabled={isSubmitting}
                >
                    Siguiente
                </Button>
            )}
        </div>
    )
}
