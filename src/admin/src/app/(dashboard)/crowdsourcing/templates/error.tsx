"use client"

import { Button } from "@/components/ui/button"
import { Alert, AlertDescription } from "@/components/ui/alert"
import { AlertCircle } from "lucide-react"

export default function TemplatesError({
    error,
    reset,
}: {
    error: Error & { digest?: string }
    reset: () => void
}) {
    return (
        <div className="space-y-6">
            <Alert variant="destructive">
                <AlertCircle className="h-4 w-4" />
                <AlertDescription>
                    Error al cargar templates: {error.message}
                </AlertDescription>
            </Alert>
            <Button onClick={reset}>Reintentar</Button>
        </div>
    )
}
