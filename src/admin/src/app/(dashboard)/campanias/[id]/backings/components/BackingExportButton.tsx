"use client"

import { useState } from "react"
import { Button } from "@/components/ui/button"
import { Download, Loader2 } from "lucide-react"
import { toast } from "sonner"
import { dashboardService } from "@/services/dashboard.service"

interface BackingExportButtonProps {
    campaniaId: string
    disabled?: boolean
}

export function BackingExportButton({
    campaniaId,
    disabled,
}: BackingExportButtonProps) {
    const [isExporting, setIsExporting] = useState(false)

    const handleExport = async () => {
        setIsExporting(true)
        try {
            const blob = await dashboardService.exportBackingsCSV(campaniaId)
            const url = window.URL.createObjectURL(blob)
            const a = document.createElement("a")
            a.href = url
            a.download = `backings-${campaniaId}.csv`
            a.click()
            window.URL.revokeObjectURL(url)
            toast.success("CSV descargado")
        } catch {
            toast.error("Error al exportar CSV")
        } finally {
            setIsExporting(false)
        }
    }

    return (
        <Button
            variant="outline"
            size="sm"
            onClick={handleExport}
            disabled={disabled || isExporting}
        >
            {isExporting ? (
                <Loader2 className="mr-2 h-4 w-4 animate-spin" />
            ) : (
                <Download className="mr-2 h-4 w-4" />
            )}
            Exportar CSV
        </Button>
    )
}
