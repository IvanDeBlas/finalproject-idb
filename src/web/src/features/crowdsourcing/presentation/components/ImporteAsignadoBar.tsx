import { FC } from "react"

interface ImporteAsignadoBarProps {
    importeAsignado: number
    importeTotal: number
    porcentaje: number
    monedaNombre: string
    importeNuevo?: number
    className?: string
}

export const ImporteAsignadoBar: FC<ImporteAsignadoBarProps> = ({
    importeAsignado,
    importeTotal,
    porcentaje,
    monedaNombre,
    importeNuevo,
    className = "",
}) => {
    const totalConNuevo = importeNuevo !== undefined
        ? importeAsignado + importeNuevo
        : importeAsignado
    const porcentajeConNuevo = importeNuevo !== undefined
        ? importeTotal > 0 ? Math.round((totalConNuevo / importeTotal) * 100) : 0
        : porcentaje
    const excedido = totalConNuevo > importeTotal

    const barWidth = Math.min(porcentajeConNuevo, 100)

    return (
        <div className={`space-y-1.5 ${className}`}>
            <div className="flex items-center justify-between text-sm">
                <span className={excedido ? "text-red-400" : "text-slate-300"}>
                    Asignado: {totalConNuevo.toLocaleString("es-ES", { minimumFractionDigits: 2 })} de{" "}
                    {importeTotal.toLocaleString("es-ES", { minimumFractionDigits: 2 })} {monedaNombre}{" "}
                    ({porcentajeConNuevo}%)
                </span>
            </div>
            <div className="h-2 w-full rounded-full bg-slate-700">
                <div
                    className={`h-full rounded-full transition-all ${
                        excedido
                            ? "bg-red-500"
                            : porcentajeConNuevo >= 100
                                ? "bg-green-500"
                                : "bg-blue-500"
                    }`}
                    style={{ width: `${barWidth}%` }}
                    role="progressbar"
                    aria-valuenow={porcentajeConNuevo}
                    aria-valuemin={0}
                    aria-valuemax={100}
                />
            </div>
            {excedido && (
                <p className="text-xs text-red-400">
                    El importe asignado supera el total pactado
                </p>
            )}
        </div>
    )
}
