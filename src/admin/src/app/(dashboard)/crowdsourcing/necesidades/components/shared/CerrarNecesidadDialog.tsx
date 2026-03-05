"use client"

import { useState } from "react"
import {
    Dialog,
    DialogContent,
    DialogHeader,
    DialogFooter,
    DialogTitle,
} from "@/components/ui/dialog"
import { Alert, AlertTitle } from "@/components/ui/alert"
import { Textarea } from "@/components/ui/textarea"
import { Button } from "@/components/ui/button"
import { Label } from "@/components/ui/label"
import { AlertCircle, Loader2 } from "lucide-react"

interface CerrarNecesidadDialogProps {
    open: boolean
    onOpenChange: (open: boolean) => void
    onConfirm: (motivo?: string) => void
    isPending: boolean
}

export function CerrarNecesidadDialog({
    open,
    onOpenChange,
    onConfirm,
    isPending,
}: CerrarNecesidadDialogProps) {
    const [motivo, setMotivo] = useState("")

    const handleConfirm = () => {
        onConfirm(motivo || undefined)
        setMotivo("")
    }

    return (
        <Dialog open={open} onOpenChange={onOpenChange}>
            <DialogContent className="max-w-md">
                <DialogHeader>
                    <DialogTitle>Cerrar Necesidad</DialogTitle>
                </DialogHeader>

                <Alert variant="destructive" className="mb-4">
                    <AlertCircle className="h-5 w-5" />
                    <AlertTitle>
                        Al cerrar esta necesidad, las propuestas pendientes seran rechazadas automaticamente.
                    </AlertTitle>
                </Alert>

                <div className="space-y-2">
                    <Label htmlFor="motivo">Motivo del cierre (opcional)</Label>
                    <Textarea
                        id="motivo"
                        value={motivo}
                        onChange={(e) => setMotivo(e.target.value)}
                        maxLength={500}
                        rows={4}
                        placeholder="Ej: Ya encontre un profesional por otra via"
                        disabled={isPending}
                    />
                    <span className="text-xs text-muted-foreground">
                        {motivo.length} / 500 caracteres
                    </span>
                </div>

                <DialogFooter className="flex gap-3">
                    <Button
                        variant="outline"
                        onClick={() => onOpenChange(false)}
                        disabled={isPending}
                    >
                        Cancelar
                    </Button>
                    <Button
                        variant="destructive"
                        onClick={handleConfirm}
                        disabled={isPending}
                    >
                        {isPending && <Loader2 className="animate-spin mr-2 h-4 w-4" />}
                        Cerrar Necesidad
                    </Button>
                </DialogFooter>
            </DialogContent>
        </Dialog>
    )
}
