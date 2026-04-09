import { FC } from "react"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Clock } from "lucide-react"
import type { AcuerdoTimelineEvento } from "../../domain"

interface AcuerdoTimelineProps {
    timeline: AcuerdoTimelineEvento[]
    className?: string
}

function formatDate(dateStr: string): string {
    return new Date(dateStr).toLocaleDateString("es-ES", {
        day: "2-digit",
        month: "short",
        year: "numeric",
        hour: "2-digit",
        minute: "2-digit",
    })
}

export const AcuerdoTimeline: FC<AcuerdoTimelineProps> = ({
    timeline,
    className = "",
}) => {
    return (
        <Card className={`bg-[#0f1729] border-[#334155] ${className}`}>
            <CardHeader className="pb-3">
                <CardTitle className="text-white text-base flex items-center gap-2">
                    <Clock className="h-4 w-4 text-slate-400" />
                    Actividad
                </CardTitle>
            </CardHeader>
            <CardContent>
                {timeline.length === 0 ? (
                    <p className="text-sm text-slate-400">No hay actividad registrada</p>
                ) : (
                    <div className="space-y-3">
                        {timeline.map((evento, index) => (
                            <div
                                key={index}
                                className="flex gap-3 text-sm"
                            >
                                <div className="flex flex-col items-center">
                                    <div className="h-2 w-2 rounded-full bg-slate-500 mt-1.5" />
                                    {index < timeline.length - 1 && (
                                        <div className="w-px flex-1 bg-slate-700" />
                                    )}
                                </div>
                                <div className="pb-3">
                                    <p className="text-slate-200">{evento.accion}</p>
                                    <p className="text-xs text-slate-500">
                                        {evento.actor} &middot; {formatDate(evento.fecha)}
                                    </p>
                                </div>
                            </div>
                        ))}
                    </div>
                )}
            </CardContent>
        </Card>
    )
}
