import { FC } from "react"
import {
    Dialog,
    DialogContent,
    DialogHeader,
    DialogTitle,
    DialogDescription,
    DialogFooter,
} from "@/components/ui/dialog"
import { Button } from "@/components/ui/button"
import { Separator } from "@/components/ui/separator"
import { CheckCircle2, AlertTriangle, Loader2 } from "lucide-react"
import { ESTADO_ENTREGABLE } from "@shared/constants"
import { useCompletarAcuerdo } from "../../application/hooks/useCompletarAcuerdo"
import type { Acuerdo } from "../../domain"

interface CompletarAcuerdoDialogProps {
    acuerdo: Acuerdo
    isOpen: boolean
    onClose: () => void
}

export const CompletarAcuerdoDialog: FC<CompletarAcuerdoDialogProps> = ({
    acuerdo,
    isOpen,
    onClose,
}) => {
    const mutation = useCompletarAcuerdo(acuerdo.id, onClose)

    const totalMilestones = acuerdo.milestones.length
    const milestonesCompletados = acuerdo.milestones.filter(
        (m) => !!m.fechaCompletado
    ).length

    const allEntregables = acuerdo.milestones.flatMap((m) => m.entregables)
    const entregablesAprobados = allEntregables.filter(
        (e) => e.estadoEntregableId === ESTADO_ENTREGABLE.APROBADO
    ).length
    const entregablesPendientes = allEntregables.filter(
        (e) => e.estadoEntregableId === ESTADO_ENTREGABLE.ENTREGADO
    ).length

    const handleConfirm = () => {
        mutation.mutate()
    }

    return (
        <Dialog open={isOpen} onOpenChange={(open) => !open && onClose()}>
            <DialogContent className="max-w-md bg-[#0f1729] border-[#334155] text-white">
                <DialogHeader>
                    <DialogTitle className="text-lg font-semibold text-white flex items-center gap-2">
                        <CheckCircle2 className="h-5 w-5 text-green-400" />
                        Completar acuerdo
                    </DialogTitle>
                    <DialogDescription className="text-sm text-slate-400">
                        Revisa el resumen antes de completar
                    </DialogDescription>
                </DialogHeader>

                <div className="space-y-3 py-2">
                    <div className="grid grid-cols-2 gap-3 text-sm">
                        <div className="bg-slate-800 rounded-lg p-3">
                            <p className="text-slate-400 text-xs">Milestones</p>
                            <p className="text-white font-medium">
                                {milestonesCompletados} / {totalMilestones} completados
                            </p>
                        </div>
                        <div className="bg-slate-800 rounded-lg p-3">
                            <p className="text-slate-400 text-xs">Entregables</p>
                            <p className="text-white font-medium">
                                {entregablesAprobados} / {allEntregables.length} aprobados
                            </p>
                        </div>
                    </div>

                    {entregablesPendientes > 0 && (
                        <>
                            <Separator className="bg-slate-700" />
                            <div className="bg-amber-900/20 border border-amber-700 rounded-lg p-3 flex items-start gap-2">
                                <AlertTriangle className="h-4 w-4 text-amber-400 shrink-0 mt-0.5" />
                                <p className="text-sm text-amber-300">
                                    Hay {entregablesPendientes} entregable
                                    {entregablesPendientes > 1 ? "s" : ""} pendiente
                                    {entregablesPendientes > 1 ? "s" : ""} de revision.
                                    Puedes completar el acuerdo igualmente.
                                </p>
                            </div>
                        </>
                    )}
                </div>

                <DialogFooter className="pt-2">
                    <Button
                        variant="outline"
                        disabled={mutation.isPending}
                        className="border-slate-600 text-white hover:bg-slate-800"
                        onClick={onClose}
                    >
                        Cancelar
                    </Button>
                    <Button
                        disabled={mutation.isPending}
                        className="bg-green-600 hover:bg-green-700"
                        onClick={handleConfirm}
                    >
                        {mutation.isPending ? (
                            <>
                                <Loader2 className="w-4 h-4 mr-2 animate-spin" />
                                Completando...
                            </>
                        ) : (
                            "Completar acuerdo"
                        )}
                    </Button>
                </DialogFooter>
            </DialogContent>
        </Dialog>
    )
}
