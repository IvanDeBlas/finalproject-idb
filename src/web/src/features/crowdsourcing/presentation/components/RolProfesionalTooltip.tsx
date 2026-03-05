import { FC } from "react"
import { Info } from "lucide-react"
import {
    Tooltip,
    TooltipContent,
    TooltipTrigger,
} from "@/components/ui/tooltip"
import type { RolProfesional } from "../../domain"

interface RolProfesionalTooltipProps {
    rol: RolProfesional
}

export const RolProfesionalTooltip: FC<RolProfesionalTooltipProps> = ({ rol }) => {
    if (!rol.descripcion) return null

    return (
        <Tooltip>
            <TooltipTrigger asChild>
                <button
                    className="inline-flex items-center justify-center w-4 h-4 rounded-full hover:bg-[#334155] transition-colors"
                    aria-label={`Informacion sobre ${rol.nombre}`}
                >
                    <Info className="w-3 h-3 text-[#94a3b8] hover:text-[#a855f7]" />
                </button>
            </TooltipTrigger>
            <TooltipContent
                side="top"
                className="max-w-xs bg-[#16213e] border-[#334155] text-white p-3"
            >
                <p className="text-sm">{rol.descripcion}</p>
                {rol.modalidadCobro && (
                    <p className="text-xs text-[#94a3b8] mt-2">
                        Modalidad: {rol.modalidadCobro}
                    </p>
                )}
            </TooltipContent>
        </Tooltip>
    )
}
