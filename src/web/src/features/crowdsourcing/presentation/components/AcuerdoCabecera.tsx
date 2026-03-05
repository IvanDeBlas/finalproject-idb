import { FC } from "react"
import { Card, CardContent } from "@/components/ui/card"
import { Separator } from "@/components/ui/separator"
import { Calendar, DollarSign, Users } from "lucide-react"
import { EstadoAcuerdoBadge } from "./EstadoAcuerdoBadge"
import { ImporteAsignadoBar } from "./ImporteAsignadoBar"
import type { Acuerdo } from "../../domain"

interface AcuerdoCabeceraProps {
    acuerdo: Acuerdo
}

function formatDate(dateStr: string): string {
    return new Date(dateStr).toLocaleDateString("es-ES", {
        day: "2-digit",
        month: "short",
        year: "numeric",
    })
}

export const AcuerdoCabecera: FC<AcuerdoCabeceraProps> = ({ acuerdo }) => {
    return (
        <Card className="bg-[#0f1729] border-[#334155]">
            <CardContent className="pt-6 space-y-4">
                <div className="flex items-start justify-between gap-3">
                    <div>
                        <h1 className="text-xl font-bold text-white">
                            {acuerdo.tituloInterno}
                        </h1>
                        <p className="text-sm text-slate-400 mt-1">
                            Necesidad: {acuerdo.necesidad.titulo}
                        </p>
                    </div>
                    <EstadoAcuerdoBadge
                        estadoId={acuerdo.estadoAcuerdoId}
                        estadoNombre={acuerdo.estadoAcuerdoNombre}
                    />
                </div>

                <Separator className="bg-slate-700" />

                <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
                    <div className="flex items-start gap-2">
                        <Users className="h-4 w-4 text-slate-400 mt-0.5" />
                        <div>
                            <p className="text-xs text-slate-400">Partes</p>
                            <p className="text-sm text-slate-200">
                                {acuerdo.artista.nombreArtistico}
                            </p>
                            <p className="text-sm text-slate-200">
                                {acuerdo.profesional.nombre}
                            </p>
                        </div>
                    </div>

                    <div className="flex items-start gap-2">
                        <DollarSign className="h-4 w-4 text-slate-400 mt-0.5" />
                        <div>
                            <p className="text-xs text-slate-400">Importe pactado</p>
                            <p className="text-sm font-medium text-white">
                                {acuerdo.importeTotalPactado.toLocaleString("es-ES", {
                                    minimumFractionDigits: 2,
                                })}{" "}
                                {acuerdo.monedaNombre}
                            </p>
                        </div>
                    </div>

                    <div className="flex items-start gap-2">
                        <Calendar className="h-4 w-4 text-slate-400 mt-0.5" />
                        <div>
                            <p className="text-xs text-slate-400">Fechas</p>
                            <p className="text-sm text-slate-200">
                                Inicio: {formatDate(acuerdo.fechaInicio)}
                            </p>
                            {acuerdo.fechaFinPrevista && (
                                <p className="text-sm text-slate-200">
                                    Fin previsto: {formatDate(acuerdo.fechaFinPrevista)}
                                </p>
                            )}
                            {acuerdo.fechaFinReal && (
                                <p className="text-sm text-slate-200">
                                    Fin real: {formatDate(acuerdo.fechaFinReal)}
                                </p>
                            )}
                        </div>
                    </div>
                </div>

                <Separator className="bg-slate-700" />

                <ImporteAsignadoBar
                    importeAsignado={acuerdo.importeAsignado}
                    importeTotal={acuerdo.importeTotalPactado}
                    porcentaje={acuerdo.porcentajeAsignado}
                    monedaNombre={acuerdo.monedaNombre}
                />
            </CardContent>
        </Card>
    )
}
