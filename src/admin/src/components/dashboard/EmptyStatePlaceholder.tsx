"use client"

import Link from "next/link"
import { Button } from "@/components/ui/button"
import type { LucideIcon } from "lucide-react"

interface EmptyStatePlaceholderProps {
    icon: LucideIcon
    title: string
    description: string
    action?: { label: string; href: string }
    className?: string
}

export function EmptyStatePlaceholder({
    icon: Icon,
    title,
    description,
    action,
    className,
}: EmptyStatePlaceholderProps) {
    return (
        <div className={`text-center py-12 ${className || ""}`}>
            <Icon className="h-12 w-12 text-muted-foreground mx-auto mb-4" />
            <p className="text-lg font-medium mb-2">{title}</p>
            <p className="text-sm text-muted-foreground mb-6 max-w-md mx-auto">
                {description}
            </p>
            {action && (
                <Link href={action.href}>
                    <Button>{action.label}</Button>
                </Link>
            )}
        </div>
    )
}
