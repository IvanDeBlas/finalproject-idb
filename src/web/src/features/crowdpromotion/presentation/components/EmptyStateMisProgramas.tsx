import type { FC } from "react"
import { Button } from "@/components/ui/button"
import { Megaphone } from "lucide-react"

interface EmptyStateMisProgramasProps {
    hasActiveEstadoFilter: boolean
    onClearFilter: () => void
    onExplorar: () => void
}

export const EmptyStateMisProgramas: FC<EmptyStateMisProgramasProps> = ({
    hasActiveEstadoFilter,
    onClearFilter,
    onExplorar,
}) => {
    if (hasActiveEstadoFilter) {
        return (
            <div className="text-center py-12" role="status">
                <p className="text-sm text-[#94a3b8] mb-4">
                    No tienes inscripciones con este estado
                </p>
                <Button
                    variant="outline"
                    size="sm"
                    onClick={onClearFilter}
                    className="border-[#334155] text-[#94a3b8] hover:text-white hover:bg-[#1e1e38]"
                >
                    Ver todas
                </Button>
            </div>
        )
    }

    return (
        <div className="text-center py-16" role="status">
            <div className="mx-auto w-20 h-20 rounded-full bg-gradient-to-br from-pink-500/20 to-purple-600/20 flex items-center justify-center mb-6">
                <Megaphone className="w-10 h-10 text-[#a855f7]" aria-hidden="true" />
            </div>
            <h3 className="text-lg font-semibold text-white mb-2">
                No estas inscrito en ningun programa
            </h3>
            <p className="text-sm text-[#94a3b8] mb-6">
                Explora los programas disponibles y empieza a ganar comisiones
            </p>
            <Button
                onClick={onExplorar}
                className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white"
            >
                Explorar programas
            </Button>
        </div>
    )
}
