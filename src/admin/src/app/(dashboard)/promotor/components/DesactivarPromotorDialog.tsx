"use client"

import {
    AlertDialog,
    AlertDialogAction,
    AlertDialogCancel,
    AlertDialogContent,
    AlertDialogDescription,
    AlertDialogFooter,
    AlertDialogHeader,
    AlertDialogTitle,
} from "@/components/ui/alert-dialog"
import { AlertTriangle } from "lucide-react"
import { useDesactivarPromotor } from "@/hooks/use-promotor-mutations"

interface DesactivarPromotorDialogProps {
    open: boolean
    onOpenChange: (open: boolean) => void
    totalProgramasActivos: number
}

export function DesactivarPromotorDialog({
    open,
    onOpenChange,
    totalProgramasActivos,
}: DesactivarPromotorDialogProps) {
    const desactivarMutation = useDesactivarPromotor()

    const handleConfirm = () => {
        desactivarMutation.mutate(undefined, {
            onSuccess: () => {
                onOpenChange(false)
            },
        })
    }

    return (
        <AlertDialog open={open} onOpenChange={onOpenChange}>
            <AlertDialogContent className="max-w-md">
                <AlertDialogHeader>
                    <AlertDialogTitle className="flex items-center gap-2">
                        {totalProgramasActivos > 0 && (
                            <AlertTriangle className="h-5 w-5 text-red-400" aria-hidden="true" />
                        )}
                        Desactivar perfil de promotor
                    </AlertDialogTitle>
                    <AlertDialogDescription>
                        {totalProgramasActivos > 0 ? (
                            <>
                                Tienes <strong>{totalProgramasActivos}</strong> programa{totalProgramasActivos !== 1 ? "s" : ""} activo{totalProgramasActivos !== 1 ? "s" : ""}.
                                Al desactivar tu perfil, todos tus programas seran dados de baja automaticamente.
                                Esta accion no se puede deshacer.
                            </>
                        ) : (
                            <>
                                Al desactivar tu perfil de promotor dejaras de ser visible para los artistas.
                                Esta accion no se puede deshacer.
                            </>
                        )}
                    </AlertDialogDescription>
                </AlertDialogHeader>
                <AlertDialogFooter>
                    <AlertDialogCancel disabled={desactivarMutation.isPending}>
                        Cancelar
                    </AlertDialogCancel>
                    <AlertDialogAction
                        onClick={handleConfirm}
                        disabled={desactivarMutation.isPending}
                        className="bg-red-600 hover:bg-red-700 focus:ring-red-600"
                    >
                        {desactivarMutation.isPending ? "Desactivando..." : "Desactivar"}
                    </AlertDialogAction>
                </AlertDialogFooter>
            </AlertDialogContent>
        </AlertDialog>
    )
}
