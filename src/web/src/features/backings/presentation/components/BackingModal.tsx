import { useState } from "react"
import {
    Dialog,
    DialogContent,
    DialogHeader,
    DialogTitle,
    DialogDescription,
} from "@/components/ui/dialog"
interface RewardForBacking {
    id: string
    nombre: string
    descripcion?: string
    importeMinimo: number
}
import type { CreateBackingFormData } from "@shared/schemas/backing.schema"
import { useCreateBacking } from "../../application/useBackings"
import { BackingForm } from "./BackingForm"

interface BackingModalProps {
    open: boolean
    onOpenChange: (open: boolean) => void
    campaniaId: string
    campaniaTitulo: string
    reward: RewardForBacking | null
    onSuccess?: () => void
}

export function BackingModal({
    open,
    onOpenChange,
    campaniaId,
    campaniaTitulo,
    reward: initialReward,
    onSuccess,
}: BackingModalProps) {
    const [selectedReward] = useState<RewardForBacking | null>(initialReward)
    const createBacking = useCreateBacking()

    const handleSubmit = (data: CreateBackingFormData & { campaniaId: string }) => {
        createBacking.mutate(data, {
            onSuccess: () => {
                onOpenChange(false)
                onSuccess?.()
            },
        })
    }

    return (
        <Dialog open={open} onOpenChange={onOpenChange}>
            <DialogContent className="max-w-2xl max-h-[90vh] overflow-y-auto">
                <DialogHeader>
                    <DialogTitle>Apoya a {campaniaTitulo}</DialogTitle>
                    <DialogDescription>
                        Completa el formulario para realizar tu aporte
                    </DialogDescription>
                </DialogHeader>

                <BackingForm
                    campaniaId={campaniaId}
                    reward={selectedReward}
                    onSubmit={handleSubmit}
                    isSubmitting={createBacking.isPending}
                    onCancel={() => onOpenChange(false)}
                />

                {createBacking.isError && (
                    <p className="text-sm text-red-500 text-center">
                        {(createBacking.error as Error)?.message ||
                            "Error al procesar tu apoyo. Intenta nuevamente."}
                    </p>
                )}
            </DialogContent>
        </Dialog>
    )
}
