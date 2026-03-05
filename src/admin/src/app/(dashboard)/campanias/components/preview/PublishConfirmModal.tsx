"use client"

import { Loader2, AlertTriangle } from "lucide-react"
import { Button } from "@/components/ui"
import {
    Dialog,
    DialogContent,
    DialogHeader,
    DialogTitle,
    DialogDescription,
    DialogFooter,
} from "@/components/ui/dialog"
import { Alert, AlertDescription } from "@/components/ui/alert"

interface PublishConfirmModalProps {
    isOpen: boolean
    onClose: () => void
    onConfirm: () => void
    hasRewards: boolean
    isPublishing?: boolean
}

export function PublishConfirmModal({
    isOpen,
    onClose,
    onConfirm,
    hasRewards,
    isPublishing,
}: PublishConfirmModalProps) {
    return (
        <Dialog open={isOpen} onOpenChange={onClose}>
            <DialogContent className="bg-card border-border sm:max-w-md">
                <DialogHeader>
                    <DialogTitle className="text-foreground">
                        {hasRewards
                            ? "Publicar campania"
                            : "Publicar sin recompensas"}
                    </DialogTitle>
                    <DialogDescription className="text-muted-foreground">
                        {hasRewards
                            ? "Una vez publicada, la campania sera visible publicamente y no podras editar algunos campos."
                            : "Tu campania no tiene recompensas. Las recompensas ayudan a incentivar a tus fans a apoyarte."}
                    </DialogDescription>
                </DialogHeader>

                {!hasRewards && (
                    <Alert className="bg-yellow-500/10 border-yellow-500/30">
                        <AlertTriangle className="h-4 w-4 text-yellow-500" />
                        <AlertDescription className="text-sm text-yellow-400">
                            Podras agregar recompensas despues, pero es
                            recomendable tener al menos una antes de publicar.
                        </AlertDescription>
                    </Alert>
                )}

                <DialogFooter className="gap-2 sm:gap-0">
                    <Button
                        variant="outline"
                        onClick={onClose}
                        disabled={isPublishing}
                        className="border-border"
                    >
                        Cancelar
                    </Button>
                    <Button
                        onClick={onConfirm}
                        disabled={isPublishing}
                        className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white"
                        aria-busy={isPublishing}
                    >
                        {isPublishing ? (
                            <>
                                <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                                Publicando...
                            </>
                        ) : hasRewards ? (
                            "Publicar"
                        ) : (
                            "Publicar sin recompensas"
                        )}
                    </Button>
                </DialogFooter>
            </DialogContent>
        </Dialog>
    )
}
