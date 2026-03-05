"use client"

import { type ReactNode } from "react"
import { Button } from "@/components/ui"

interface EmptyStateProps {
    icon: ReactNode
    title: string
    description: string
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
        <div className="text-center py-16">
            <div className="text-muted-foreground mx-auto mb-4 flex justify-center">
                {icon}
            </div>

            <h3 className="text-lg font-semibold text-foreground mb-2">
                {title}
            </h3>

            <p className="text-sm text-muted-foreground mb-6 max-w-sm mx-auto">
                {description}
            </p>

            {actionLabel && onAction && (
                <Button
                    onClick={onAction}
                    className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold"
                >
                    {actionLabel}
                </Button>
            )}
        </div>
    )
}
