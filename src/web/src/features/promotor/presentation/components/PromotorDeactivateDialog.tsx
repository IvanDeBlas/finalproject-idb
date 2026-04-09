import type { FC } from "react"
import {
    Dialog,
    DialogContent,
    DialogDescription,
    DialogFooter,
    DialogHeader,
    DialogTitle,
} from "@/components/ui/dialog"
import { Button } from "@/components/ui/button"
import { Separator } from "@/components/ui/separator"
import { AlertTriangle, Loader2 } from "lucide-react"

interface PromotorDeactivateDialogProps {
    open: boolean
    onOpenChange: (open: boolean) => void
    onConfirm: () => Promise<void>
    isConfirming: boolean
    totalProgramasActivos: number
}

export const PromotorDeactivateDialog: FC<PromotorDeactivateDialogProps> = ({
    open,
    onOpenChange,
    onConfirm,
    isConfirming,
    totalProgramasActivos,
}) => {
    return (
        <Dialog open={open} onOpenChange={onOpenChange}>
            <DialogContent className="bg-[#151525] border-[#334155] text-white sm:max-w-md">
                <DialogHeader>
                    <div className="flex items-center gap-2">
                        <AlertTriangle className="w-5 h-5 text-amber-400" />
                        <DialogTitle className="text-white">Desactivar cuenta de promotor</DialogTitle>
                    </div>
                    <Separator className="bg-[#334155] my-2" />
                    <DialogDescription className="text-[#94a3b8]">
                        {totalProgramasActivos > 0 ? (
                            <>
                                Tienes <span className="font-semibold text-amber-400">{totalProgramasActivos}</span>{" "}
                                {totalProgramasActivos === 1 ? "programa activo" : "programas activos"} que{" "}
                                {totalProgramasActivos === 1 ? "sera dado" : "seran dados"} de baja
                                al desactivar tu cuenta. Esta accion no se puede deshacer facilmente.
                            </>
                        ) : (
                            "Al desactivar tu cuenta de promotor, tu perfil dejara de estar visible. Esta accion no se puede deshacer facilmente."
                        )}
                    </DialogDescription>
                </DialogHeader>
                <DialogFooter className="gap-2 sm:gap-0">
                    <Button
                        variant="outline"
                        className="border-[#334155] text-[#cbd5e1] hover:bg-[#1e1e38] hover:text-white"
                        onClick={() => onOpenChange(false)}
                        disabled={isConfirming}
                    >
                        Cancelar
                    </Button>
                    <Button
                        variant="destructive"
                        onClick={onConfirm}
                        disabled={isConfirming}
                        className="bg-red-600 hover:bg-red-700"
                    >
                        {isConfirming ? (
                            <>
                                <Loader2 className="w-4 h-4 animate-spin mr-2" />
                                Desactivando...
                            </>
                        ) : (
                            "Desactivar cuenta"
                        )}
                    </Button>
                </DialogFooter>
            </DialogContent>
        </Dialog>
    )
}
