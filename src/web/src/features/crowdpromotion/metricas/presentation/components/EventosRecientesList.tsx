import { Link } from "react-router-dom"
import { Activity, ChevronRight } from "lucide-react"
import { Card } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Skeleton } from "@/components/ui/skeleton"
import type { EventoReciente } from "../../domain"

const TRACKING_TIPO_BACKING = 4
import { EventoTipoBadge } from "./EventoTipoBadge"

interface EventosRecientesListProps {
    eventos: EventoReciente[]
    programaId: string | undefined
    isLoading?: boolean
}

function formatFechaEvento(fechaIso: string): string {
    const date = new Date(fechaIso)
    return new Intl.DateTimeFormat("es-ES", {
        day: "numeric",
        month: "short",
        year: "numeric",
        hour: "2-digit",
        minute: "2-digit",
    }).format(date)
}

function EventoRow({ evento }: { evento: EventoReciente }) {
    return (
        <div
            className="flex items-center gap-3 px-5 py-3.5 hover:bg-[#1e1e38] transition-colors duration-150"
            role="listitem"
        >
            <EventoTipoBadge tipoEventoId={evento.tipoEventoId} />

            <div className="flex-1 min-w-0">
                <p className="text-sm text-white truncate">
                    {evento.tipoEventoNombre}
                </p>
                {evento.tipoEventoId === TRACKING_TIPO_BACKING && evento.valorMonetario > 0 && (
                    <p className="text-xs text-[#94a3b8]">
                        {evento.valorMonetario} {evento.monedaNombre ?? "EUR"}
                        {evento.comisionGenerada != null && (
                            <> - {evento.comisionGenerada} {evento.monedaNombre ?? "EUR"} comision</>
                        )}
                    </p>
                )}
            </div>

            <span className="text-xs text-[#64748b] shrink-0">
                {formatFechaEvento(evento.fechaEvento)}
            </span>
        </div>
    )
}

function EmptyStateEventos() {
    return (
        <div className="flex flex-col items-center justify-center py-10 text-center px-5">
            <Activity className="w-8 h-8 text-[#64748b] mb-3" />
            <p className="text-sm text-[#94a3b8]">
                Aun no hay eventos registrados.
            </p>
            <p className="text-sm text-[#64748b]">
                Comparte tu enlace de promocion para empezar.
            </p>
        </div>
    )
}

function EventosRecientesSkeleton() {
    return (
        <div className="bg-[#151525] border border-[#334155] rounded-lg">
            <div className="flex items-center justify-between p-5 border-b border-[#334155]">
                <Skeleton className="w-36 h-5 bg-[#1e1e38]" />
                <Skeleton className="w-20 h-6 bg-[#1e1e38] rounded-md" />
            </div>
            <div className="divide-y divide-[#1e1e38]">
                {[0, 1, 2].map(i => (
                    <div key={i} className="flex items-center gap-3 px-5 py-3.5">
                        <Skeleton className="w-14 h-5 rounded-full bg-[#1e1e38]" />
                        <Skeleton className="flex-1 h-4 bg-[#1e1e38]" />
                        <Skeleton className="w-24 h-4 bg-[#1e1e38] shrink-0" />
                    </div>
                ))}
            </div>
        </div>
    )
}

export function EventosRecientesList({ eventos, programaId, isLoading }: EventosRecientesListProps) {
    if (isLoading) {
        return <EventosRecientesSkeleton />
    }

    return (
        <Card className="bg-[#151525] border-[#334155]">
            <div className="flex items-center justify-between p-5 border-b border-[#334155]">
                <h3 className="text-base font-semibold text-white">Eventos recientes</h3>
                <Button
                    variant="ghost"
                    size="sm"
                    asChild
                    className="text-[#a855f7] hover:text-[#c084fc] text-xs focus-visible:ring-[#a855f7] focus-visible:ring-offset-[#1a1a2e]"
                >
                    <Link to={`/promotor/eventos${programaId ? `?programaId=${programaId}` : ""}`}>
                        Ver todos
                        <ChevronRight className="w-3.5 h-3.5 ml-0.5" />
                    </Link>
                </Button>
            </div>

            {eventos.length === 0 ? (
                <EmptyStateEventos />
            ) : (
                <div className="divide-y divide-[#1e1e38]" role="list">
                    {eventos.map(evento => (
                        <EventoRow key={evento.id} evento={evento} />
                    ))}
                </div>
            )}
        </Card>
    )
}
