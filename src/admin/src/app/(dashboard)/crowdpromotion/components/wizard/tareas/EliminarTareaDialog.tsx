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

interface EliminarTareaDialogProps {
    open: boolean
    onOpenChange: (open: boolean) => void
    onConfirmar: () => void
    nombreTarea: string
}

export function EliminarTareaDialog({
    open,
    onOpenChange,
    onConfirmar,
    nombreTarea,
}: EliminarTareaDialogProps) {
    return (
        <AlertDialog open={open} onOpenChange={onOpenChange}>
            <AlertDialogContent className="bg-[#1a1a2e] border-zinc-800">
                <AlertDialogHeader>
                    <AlertDialogTitle className="text-white">Eliminar tarea</AlertDialogTitle>
                    <AlertDialogDescription className="text-zinc-400">
                        ¿Estas seguro de eliminar la tarea <strong className="text-white">&quot;{nombreTarea}&quot;</strong>?
                        Esta accion no se puede deshacer.
                    </AlertDialogDescription>
                </AlertDialogHeader>
                <AlertDialogFooter>
                    <AlertDialogCancel>Cancelar</AlertDialogCancel>
                    <AlertDialogAction
                        className="bg-red-600 hover:bg-red-700"
                        onClick={onConfirmar}
                    >
                        Eliminar
                    </AlertDialogAction>
                </AlertDialogFooter>
            </AlertDialogContent>
        </AlertDialog>
    )
}
