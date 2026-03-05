"use client"

import { Badge } from "@/components/ui/badge"
import { MapPin, Wifi, Globe } from "lucide-react"
import { MODALIDAD_TRABAJO, MODALIDAD_TRABAJO_LABELS } from "@shared/constants"

interface ModalidadBadgeProps {
    modalidadId: number
}

const MODALIDAD_ICONS: Record<number, typeof MapPin> = {
    [MODALIDAD_TRABAJO.PRESENCIAL]: MapPin,
    [MODALIDAD_TRABAJO.REMOTO]: Wifi,
    [MODALIDAD_TRABAJO.HIBRIDO]: Globe,
}

export function ModalidadBadge({ modalidadId }: ModalidadBadgeProps) {
    const label = MODALIDAD_TRABAJO_LABELS[modalidadId] || "Desconocido"
    const Icon = MODALIDAD_ICONS[modalidadId] || Globe

    return (
        <Badge variant="secondary" className="gap-1">
            <Icon className="h-3 w-3" />
            {label}
        </Badge>
    )
}
