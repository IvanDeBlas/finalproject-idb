"use client"

import { AlertTriangle, Loader2 } from "lucide-react"
import {
    AlertDialog,
    AlertDialogContent,
    AlertDialogHeader,
    AlertDialogTitle,
    AlertDialogDescription,
    AlertDialogFooter,
    AlertDialogCancel,
    AlertDialogAction,
} from "@/components/ui/alert-dialog"
import { useDeleteReward } from "@/hooks/use-rewards"
import { toast } from "sonner"
import type { Reward } from "@shared/types"

interface RewardDeleteDialogProps {
    isOpen: boolean
    onClose: () => void
    reward: Reward | null
}

export function RewardDeleteDialog({
    isOpen,
    onClose,
    reward,
}: RewardDeleteDialogProps) {
    const { mutate: deleteReward, isPending } = useDeleteReward()

    if (!reward) return null

    const handleDelete = () => {
        deleteReward(reward.id, {
            onSuccess: () => {
                toast.success("Recompensa eliminada exitosamente")
                onClose()
            },
            onError: (error: unknown) => {
                const err = error as { errorCode?: string }
                if (err?.errorCode === "4010") {
                    toast.error(
                        "Esta recompensa tiene aportes y no puede ser eliminada. Desactivala en su lugar."
                    )
                } else {
                    toast.error("Error al eliminar recompensa")
                }
            },
        })
    }

    return (
        <AlertDialog open={isOpen} onOpenChange={onClose}>
            <AlertDialogContent className="bg-card border-border max-w-md">
                <AlertDialogHeader className="border-b border-border pb-4 mb-4">
                    <AlertDialogTitle className="flex items-center gap-2 text-xl font-bold text-foreground">
                        <AlertTriangle className="w-6 h-6 text-amber-500" />
                        Eliminar Recompensa
                    </AlertDialogTitle>
                </AlertDialogHeader>

                <AlertDialogDescription asChild>
                    <div className="text-muted-foreground space-y-3">
                        <p>
                            ¿Estas seguro que deseas eliminar esta
                            recompensa?
                        </p>
                        <p className="font-semibold text-foreground bg-background px-3 py-2 rounded border border-border">
                            {reward.nombre} (&euro;
                            {reward.importeMinimo.toFixed(2)})
                        </p>
                        <p className="text-sm text-muted-foreground italic">
                            Esta accion no se puede deshacer.
                        </p>
                    </div>
                </AlertDialogDescription>

                <AlertDialogFooter className="border-t border-border pt-4 mt-6 flex gap-3 justify-end">
                    <AlertDialogCancel
                        disabled={isPending}
                        className="border-border text-muted-foreground hover:text-foreground hover:bg-secondary"
                    >
                        Cancelar
                    </AlertDialogCancel>
                    <AlertDialogAction
                        onClick={handleDelete}
                        disabled={isPending}
                        className="bg-red-500 hover:bg-red-600 text-white font-semibold"
                    >
                        {isPending && (
                            <Loader2 className="w-4 h-4 mr-2 animate-spin" />
                        )}
                        {isPending ? "Eliminando..." : "Eliminar"}
                    </AlertDialogAction>
                </AlertDialogFooter>
            </AlertDialogContent>
        </AlertDialog>
    )
}
