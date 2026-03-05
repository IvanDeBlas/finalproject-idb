import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { TIPO_PROMO_LABELS } from "@shared/constants"
import type { PromoProgramaDetail } from "@shared/types"

interface PromoProgramaInfoTabProps {
    programa: PromoProgramaDetail
}

function InfoRow({ label, value }: { label: string; value: string | null | undefined }) {
    return (
        <div className="flex flex-col gap-1">
            <span className="text-xs text-zinc-500 uppercase tracking-wider">{label}</span>
            {value ? (
                <span className="text-sm text-white">{value}</span>
            ) : (
                <span className="text-sm text-zinc-500 italic">--</span>
            )}
        </div>
    )
}

export function PromoProgramaInfoTab({ programa }: PromoProgramaInfoTabProps) {
    const fechaInicioTexto = programa.fechaInicio
        ? new Date(programa.fechaInicio).toLocaleDateString("es-ES")
        : null
    const fechaFinTexto = programa.fechaFin
        ? new Date(programa.fechaFin).toLocaleDateString("es-ES")
        : null
    const periodoTexto = fechaInicioTexto || fechaFinTexto
        ? [fechaInicioTexto, fechaFinTexto].filter(Boolean).join(" - ")
        : null

    const comisionPorcentaje = programa.importeComisionPorcentaje != null
        ? `${programa.importeComisionPorcentaje}%`
        : null
    const comisionFija = programa.importeComisionFija != null
        ? `${programa.importeComisionFija} ${programa.monedaNombre}`
        : null

    return (
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
            <Card className="bg-[#1a1a2e] border-zinc-800">
                <CardHeader>
                    <CardTitle className="text-base text-white">Datos del programa</CardTitle>
                </CardHeader>
                <CardContent className="space-y-4">
                    <InfoRow label="Titulo" value={programa.titulo} />
                    <InfoRow label="Descripcion" value={programa.descripcion} />
                    <InfoRow label="Tipo de programa" value={TIPO_PROMO_LABELS[programa.tipoPromoId] || programa.tipoPromoNombre} />
                    <InfoRow label="Campana vinculada" value={programa.campaniaTitulo} />
                    <InfoRow label="Periodo" value={periodoTexto} />
                </CardContent>
            </Card>

            <Card className="bg-[#1a1a2e] border-zinc-800">
                <CardHeader>
                    <CardTitle className="text-base text-white">Configuracion de comisiones</CardTitle>
                </CardHeader>
                <CardContent className="space-y-4">
                    <InfoRow label="Moneda" value={programa.monedaNombre} />
                    <InfoRow label="Comision porcentaje" value={comisionPorcentaje} />
                    <InfoRow label="Comision fija" value={comisionFija} />
                    <InfoRow label="URL Landing" value={programa.urlLanding} />
                    <InfoRow label="Codigo tracking" value={programa.codigoTrackingBase} />
                </CardContent>
            </Card>
        </div>
    )
}
