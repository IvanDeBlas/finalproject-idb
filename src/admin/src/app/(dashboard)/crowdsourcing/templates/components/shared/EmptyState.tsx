"use client"

import type { ReactNode } from "react"
import { Button } from "@/components/ui/button"

interface EmptyStateProps {
    icon: ReactNode
    title: string
    description?: string
    actionLabel?: string
    onAction?: () => void
}

export function EmptyState({
    icon,
    title,
    description,
    actionLabel,
    onAction,
}: EmptyStateProps) {
    return (
        <div className="flex flex-col items-center justify-center py-16">
            <div className="text-6xl mb-4">{icon}</div>
            <p className="text-lg text-muted-foreground mb-2">{title}</p>
            {description && (
                <p className="text-sm text-muted-foreground/60 mb-6">{description}</p>
            )}
            {actionLabel && onAction && (
                <Button onClick={onAction}>{actionLabel}</Button>
            )}
        </div>
    )
}
