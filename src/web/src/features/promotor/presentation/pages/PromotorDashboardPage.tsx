import { Navigate, Link } from "react-router-dom"
import { Alert, AlertDescription } from "@/components/ui/alert"
import { Button } from "@/components/ui/button"
import { Activity, TrendingUp, Wallet, AlertCircle } from "lucide-react"
import { APP_ROUTES } from "@shared/constants"
import { formatComisionesGanadas } from "@shared/utils/format"
import { usePromotor } from "../../application"
import { PromotorKpiCard } from "../components/PromotorKpiCard"
import { PromotorPerfilCard } from "../components/PromotorPerfilCard"

export default function PromotorDashboardPage() {
    const { data: promotor, isLoading, isError } = usePromotor()

    // Guard: no profile -> redirect to registration
    if (!isLoading && isError) {
        return <Navigate to={APP_ROUTES.landing.promotor.registro} replace />
    }

    return (
        <div className="p-6 space-y-6 bg-[#1a1a2e] min-h-screen">
            {/* Banner inactivo */}
            {promotor && !promotor.esActivo && (
                <Alert className="bg-amber-950/40 border border-amber-800/50">
                    <AlertCircle className="h-4 w-4 text-amber-400" />
                    <AlertDescription className="text-amber-400/80">
                        Tu perfil de promotor esta desactivado. Contacta soporte para reactivarlo.
                    </AlertDescription>
                </Alert>
            )}

            {/* Header */}
            <header className="flex items-center justify-between">
                <div>
                    <h1 className="text-2xl font-bold text-white">
                        {isLoading ? "Cargando..." : `Hola, ${promotor?.nombrePublico}`}
                    </h1>
                    <p className="text-sm text-[#94a3b8] mt-0.5">
                        {new Date().toLocaleDateString("es-ES", {
                            weekday: "long",
                            day: "numeric",
                            month: "long",
                            year: "numeric",
                        })}
                    </p>
                </div>
                <Button
                    className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold h-10 px-5"
                    asChild
                >
                    <Link to={APP_ROUTES.landing.promotor.perfil}>Editar perfil</Link>
                </Button>
            </header>

            {/* KPI Cards */}
            <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
                <PromotorKpiCard
                    icon={Activity}
                    iconColorClass="text-[#10b981]"
                    iconBgClass="bg-emerald-950/40"
                    label="Programas activos"
                    value={promotor?.totalProgramasActivos ?? 0}
                    isLoading={isLoading}
                />
                <PromotorKpiCard
                    icon={TrendingUp}
                    iconColorClass="text-[#3b82f6]"
                    iconBgClass="bg-blue-950/40"
                    label="Comisiones ganadas"
                    value={promotor ? formatComisionesGanadas(promotor.totalComisionesGanadas, promotor.monedaComisiones) : "0,00 \u20AC"}
                    isLoading={isLoading}
                />
                <PromotorKpiCard
                    icon={Wallet}
                    iconColorClass="text-[#a855f7]"
                    iconBgClass="bg-purple-950/40"
                    label="Saldo disponible"
                    value={promotor ? formatComisionesGanadas(promotor.totalComisionesGanadas, promotor.monedaComisiones) : "0,00 \u20AC"}
                    isLoading={isLoading}
                />
            </div>

            {/* Perfil Card */}
            <PromotorPerfilCard promotor={promotor} isLoading={isLoading} />
        </div>
    )
}
