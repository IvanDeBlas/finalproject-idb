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
import { useDesactivarPromoPrograma } from "@/hooks/use-promo-programas-mutations"

interface DesactivarPromoProgramaDialogProps {
    open: boolean
    onOpenChange: (open: boolean) => void
    programaId: string
    tituloPrograma: string
}

export function DesactivarPromoProgramaDialog({
    open,
    onOpenChange,
    programaId,
    tituloPrograma,
}: DesactivarPromoProgramaDialogProps) {
    const desactivarMutation = useDesactivarPromoPrograma()

    const handleConfirmar = () => {
        desactivarMutation.mutate(programaId, {
            onSuccess: () => {
                onOpenChange(false)
            },
        })
    }

    return (
        <AlertDialog open={open} onOpenChange={onOpenChange}>
            <AlertDialogContent className="bg-[#1a1a2e] border-zinc-800">
                <AlertDialogHeader>
                    <AlertDialogTitle className="text-white">Desactivar programa</AlertDialogTitle>
                    <AlertDialogDescription className="text-zinc-400">
                        Estas a punto de desactivar <strong className="text-white">&quot;{tituloPrograma}&quot;</strong>.
                        Todas las tareas activas tambien se desactivaran y el programa dejara de aparecer
                        en el catalogo publico.
                    </AlertDialogDescription>
                </AlertDialogHeader>
                <AlertDialogFooter>
                    <AlertDialogCancel disabled={desactivarMutation.isPending}>
                        Cancelar
                    </AlertDialogCancel>
                    <AlertDialogAction
                        className="bg-red-600 hover:bg-red-700"
                        onClick={handleConfirmar}
                        disabled={desactivarMutation.isPending}
                    >
                        {desactivarMutation.isPending ? "Desactivando..." : "Desactivar"}
                    </AlertDialogAction>
                </AlertDialogFooter>
            </AlertDialogContent>
        </AlertDialog>
    )
}
