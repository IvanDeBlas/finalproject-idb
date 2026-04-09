"use client"

import { useState } from "react"
import { AlertCircle, X } from "lucide-react"
import { Alert, AlertDescription } from "@/components/ui/alert"
import { Button } from "@/components/ui"

interface DraftBannerProps {
    onClose?: () => void
}

export function DraftBanner({ onClose }: DraftBannerProps) {
    const [isVisible, setIsVisible] = useState(true)

    if (!isVisible) return null

    const handleClose = () => {
        setIsVisible(false)
        onClose?.()
    }

    return (
        <Alert className="bg-yellow-500/10 border-yellow-500 text-yellow-500 mb-4 relative">
            <AlertCircle className="h-4 w-4" />
            <AlertDescription className="font-medium pr-8">
                VISTA PREVIA - Campania en borrador (No visible publicamente)
            </AlertDescription>
            <Button
                variant="ghost"
                size="icon"
                onClick={handleClose}
                className="absolute right-2 top-2 h-6 w-6 text-yellow-500 hover:text-yellow-400 hover:bg-yellow-500/10"
            >
                <X className="h-4 w-4" />
            </Button>
        </Alert>
    )
}
