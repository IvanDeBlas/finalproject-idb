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
import { AlertTriangle, Loader2 } from "lucide-react"
import { useRetirarPropuesta } from "../../application"
import type { MiPropuestaList } from "../../domain/types"

interface RetirarPropuestaDialogProps {
    propuesta: MiPropuestaList | null
    isOpen: boolean
    onClose: () => void
}

export const RetirarPropuestaDialog: FC<RetirarPropuestaDialogProps> = ({
    propuesta,
    isOpen,
    onClose,
}) => {
    const { mutate, isPending } = useRetirarPropuesta()

    const handleConfirm = () => {
        if (!propuesta) return
        mutate(propuesta.id, {
            onSuccess: () => onClose(),
        })
    }

    return (
        <Dialog open={isOpen} onOpenChange={(open) => !open && onClose()}>
            <DialogContent className="max-w-md bg-[#0f1729] border-[#334155] text-white">
                <DialogHeader>
                    <DialogTitle className="text-lg font-semibold text-white flex items-center gap-2">
                        <AlertTriangle className="w-5 h-5 text-red-400" aria-hidden="true" />
                        Retirar propuesta
                    </DialogTitle>
                    <DialogDescription className="text-sm text-[#94a3b8] mt-2">
                        Estas a punto de retirar tu propuesta para{" "}
                        <span className="text-white font-medium">
                            &ldquo;{propuesta?.necesidadTitulo}&rdquo;
                        </span>
                    </DialogDescription>
                </DialogHeader>

                <div className="py-4">
                    <div className="bg-red-900/20 border border-red-700 rounded-lg p-3 flex items-start gap-2">
                        <AlertTriangle className="w-4 h-4 text-red-400 flex-shrink-0 mt-0.5" aria-hidden="true" />
                        <p className="text-sm text-red-300">
                            Esta accion no se puede deshacer. Una vez retirada, no podras
                            volver a enviar una propuesta para esta necesidad.
                        </p>
                    </div>
                </div>

                <DialogFooter className="pt-2">
                    <Button
                        variant="outline"
                        disabled={isPending}
                        className="border-[#334155] text-white hover:bg-[#1e2a42]"
                        onClick={onClose}
                    >
                        Cancelar
                    </Button>
                    <Button
                        variant="destructive"
                        disabled={isPending}
                        className="bg-red-600 hover:bg-red-700"
                        onClick={handleConfirm}
                    >
                        {isPending ? (
                            <>
                                <Loader2 className="w-4 h-4 mr-2 animate-spin" aria-hidden="true" />
                                Retirando...
                            </>
                        ) : (
                            "Confirmar retirada"
                        )}
                    </Button>
                </DialogFooter>
            </DialogContent>
        </Dialog>
    )
}
