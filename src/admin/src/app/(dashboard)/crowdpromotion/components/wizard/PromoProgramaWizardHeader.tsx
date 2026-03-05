"use client"

import { Button } from "@/components/ui/button"
import { X } from "lucide-react"

interface PromoProgramaWizardHeaderProps {
    mode: "create" | "edit"
    onCancelar: () => void
}

export function PromoProgramaWizardHeader({ mode, onCancelar }: PromoProgramaWizardHeaderProps) {
    const titulo = mode === "create"
        ? "Crear programa de promocion"
        : "Editar programa de promocion"

    return (
        <div className="flex items-center justify-between h-14 px-4 bg-[#0d0d1a] border-b border-zinc-800">
            <span className="text-sm font-semibold text-purple-400">WePlay</span>
            <h2 className="text-sm font-medium text-white">{titulo}</h2>
            <Button
                variant="ghost"
                size="icon"
                className="h-8 w-8 text-zinc-400 hover:text-white"
                onClick={onCancelar}
            >
                <X className="h-4 w-4" />
            </Button>
        </div>
    )
}
