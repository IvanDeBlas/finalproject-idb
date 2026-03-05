import type { FC } from "react"
import { Button } from "@/components/ui/button"
import { AlertCircle } from "lucide-react"

interface SinPerfilPromotorBannerProps {
    onCrearPerfil: () => void
}

export const SinPerfilPromotorBanner: FC<SinPerfilPromotorBannerProps> = ({ onCrearPerfil }) => {
    return (
        <div
            className="bg-amber-950/30 border border-amber-800/50 rounded-lg p-4 flex items-center justify-between gap-4 mb-4"
            role="alert"
            aria-live="polite"
        >
            <div className="flex items-center gap-3">
                <AlertCircle className="w-5 h-5 text-amber-400 shrink-0" aria-hidden="true" />
                <p className="text-sm text-amber-300">
                    Necesitas un perfil de promotor para solicitar inscripciones.
                </p>
            </div>
            <Button
                size="sm"
                className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white text-xs shrink-0"
                onClick={onCrearPerfil}
            >
                Crear perfil
            </Button>
        </div>
    )
}
