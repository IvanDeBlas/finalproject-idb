"use client"

import {
    ResponsiveContainer,
    LineChart,
    Line,
    XAxis,
    YAxis,
    CartesianGrid,
    Tooltip,
    Legend,
} from "recharts"
import { Card, CardHeader, CardTitle } from "@/components/ui/card"
import type { EventosPorDiaItem } from "@shared/types/crowdpromotion"

interface GraficoTemporalProps {
    datos: EventosPorDiaItem[]
}

function formatFechaGrafico(fecha: string): string {
    const parts = fecha.split("-")
    if (parts.length === 3) {
        return `${parts[2]}/${parts[1]}`
    }
    return fecha
}

export function GraficoTemporal({ datos }: GraficoTemporalProps) {
    return (
        <Card className="bg-[#151525] border-zinc-800">
            <CardHeader className="pb-3">
                <CardTitle className="text-sm font-medium text-[#94a3b8]">
                    Eventos por dia
                </CardTitle>
            </CardHeader>
            <div className="px-6 pb-6">
                {datos.length === 0 ? (
                    <div className="flex items-center justify-center h-64">
                        <p className="text-sm text-[#64748b]">
                            No hay datos para el periodo seleccionado
                        </p>
                    </div>
                ) : (
                    <>
                        <ResponsiveContainer width="100%" height={256}>
                            <LineChart data={datos}>
                                <CartesianGrid
                                    stroke="#334155"
                                    strokeDasharray="3 3"
                                    opacity={0.5}
                                />
                                <XAxis
                                    dataKey="fecha"
                                    tickFormatter={formatFechaGrafico}
                                    tick={{ fill: "#64748b", fontSize: 11 }}
                                    stroke="#334155"
                                />
                                <YAxis
                                    tick={{ fill: "#64748b", fontSize: 11 }}
                                    allowDecimals={false}
                                    stroke="#334155"
                                />
                                <Tooltip
                                    contentStyle={{
                                        background: "#1e1e38",
                                        border: "1px solid #334155",
                                        borderRadius: "8px",
                                        color: "#fff",
                                    }}
                                />
                                <Legend
                                    wrapperStyle={{ fontSize: "12px", color: "#94a3b8" }}
                                />
                                <Line
                                    dataKey="clicks"
                                    stroke="#3b82f6"
                                    strokeWidth={2}
                                    dot={false}
                                    name="Clicks"
                                />
                                <Line
                                    dataKey="signups"
                                    stroke="#10b981"
                                    strokeWidth={2}
                                    dot={false}
                                    name="Signups"
                                />
                                <Line
                                    dataKey="conversiones"
                                    stroke="#a855f7"
                                    strokeWidth={2}
                                    dot={false}
                                    name="Conversiones"
                                />
                            </LineChart>
                        </ResponsiveContainer>
                        <p className="sr-only">
                            Grafico de lineas mostrando la evolucion diaria de clicks, signups y
                            conversiones durante el periodo seleccionado.
                        </p>
                    </>
                )}
            </div>
        </Card>
    )
}
