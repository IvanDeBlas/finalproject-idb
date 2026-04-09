import { FC } from "react"
import { Button } from "@/components/ui/button"
import { FileText, Filter } from "lucide-react"
import { Link } from "react-router-dom"
import { APP_ROUTES } from "@shared/constants"

interface EmptyStatePropuestasProps {
    estadoFiltroActivo: number | null
    estadoNombre?: string
}

export const EmptyStatePropuestas: FC<EmptyStatePropuestasProps> = ({
    estadoFiltroActivo,
    estadoNombre,
}) => {
    return (
        <div role="status" className="text-center py-12">
            {estadoFiltroActivo === null ? (
                <>
                    <FileText
                        className="w-12 h-12 text-[#334155] mx-auto mb-4"
                        aria-hidden="true"
                    />
                    <h3 className="text-lg font-semibold text-white mb-2">
                        Sin propuestas enviadas
                    </h3>
                    <p className="text-sm text-[#94a3b8] mb-6">
                        Aun no has enviado propuestas a ningun artista
                    </p>
                    <Link to={APP_ROUTES.landing.crowdsourcing.necesidades}>
                        <Button className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700">
                            Explorar necesidades
                        </Button>
                    </Link>
                </>
            ) : (
                <>
                    <Filter
                        className="w-12 h-12 text-[#334155] mx-auto mb-4"
                        aria-hidden="true"
                    />
                    <h3 className="text-lg font-semibold text-white mb-2">
                        Sin propuestas {estadoNombre}
                    </h3>
                    <p className="text-sm text-[#94a3b8]">
                        No tienes propuestas en este estado
                    </p>
                </>
            )}
        </div>
    )
}
