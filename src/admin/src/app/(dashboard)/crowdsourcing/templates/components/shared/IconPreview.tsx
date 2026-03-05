"use client"

import {
    Music,
    Video,
    Route,
    Speaker,
    MusicIcon,
    Palette,
    Disc,
    Mic,
    Headphones,
    Radio,
} from "lucide-react"
import { cn } from "@/lib/utils"
import type { FC } from "react"

interface IconPreviewProps {
    icon: string
    size?: "sm" | "md" | "lg"
}

const ICON_MAP: Record<string, FC<{ className?: string }>> = {
    music: Music,
    video: Video,
    route: Route,
    speaker: Speaker,
    "music-note": MusicIcon,
    palette: Palette,
    disc: Disc,
    mic: Mic,
    headphones: Headphones,
    radio: Radio,
}

const sizeClasses = {
    sm: "h-4 w-4",
    md: "h-6 w-6",
    lg: "h-10 w-10",
}

export const AVAILABLE_ICONS = Object.keys(ICON_MAP)

export function IconPreview({ icon, size = "md" }: IconPreviewProps) {
    const IconComponent = ICON_MAP[icon]

    if (!IconComponent) {
        return <span className={cn("text-muted-foreground", sizeClasses[size])}>?</span>
    }

    return <IconComponent className={cn("text-primary", sizeClasses[size])} />
}
