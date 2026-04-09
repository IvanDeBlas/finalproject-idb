import { useParams } from "react-router-dom"
import { Skeleton } from "@/components/ui/skeleton"
import { ValoracionesSection } from "../components/ValoracionesSection"
import { RatingBadge } from "../components/RatingBadge"
import { useValoracionesUsuario } from "../../application/hooks/useValoracionesUsuario"

export default function PerfilProfesionalPage() {
    const { userId } = useParams<{ userId: string }>()
    const { data, isLoading } = useValoracionesUsuario(userId ?? "")

    if (!userId) {
        return (
            <div className="max-w-3xl mx-auto px-4 py-8">
                <p className="text-[#94a3b8] text-center">
                    Usuario no encontrado
                </p>
            </div>
        )
    }

    return (
        <div className="max-w-3xl mx-auto px-4 py-8">
            <div className="mb-6">
                {isLoading ? (
                    <div className="flex items-center gap-4">
                        <Skeleton className="h-12 w-12 rounded-full bg-[#1e2a42]" />
                        <div>
                            <Skeleton className="h-6 w-48 bg-[#1e2a42] mb-2" />
                            <Skeleton className="h-4 w-32 bg-[#1e2a42]" />
                        </div>
                    </div>
                ) : (
                    <div className="flex items-center gap-4">
                        <div className="w-12 h-12 rounded-full bg-[#334155] flex items-center justify-center text-white font-semibold text-lg">
                            P
                        </div>
                        <div>
                            <h1 className="text-2xl font-bold text-white">
                                Perfil profesional
                            </h1>
                            <RatingBadge
                                puntuacionMedia={
                                    data?.resumen.puntuacionMedia ?? null
                                }
                                totalValoraciones={
                                    data?.resumen.totalValoraciones ?? 0
                                }
                                variant="medium"
                                isLoading={isLoading}
                            />
                        </div>
                    </div>
                )}
            </div>

            <ValoracionesSection userId={userId} />
        </div>
    )
}
