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

interface PromoProgramaAbandonDialogProps {
    open: boolean
    onOpenChange: (open: boolean) => void
    onConfirmar: () => void
}

export function PromoProgramaAbandonDialog({
    open,
    onOpenChange,
    onConfirmar,
}: PromoProgramaAbandonDialogProps) {
    return (
        <AlertDialog open={open} onOpenChange={onOpenChange}>
            <AlertDialogContent className="bg-[#1a1a2e] border-zinc-800">
                <AlertDialogHeader>
                    <AlertDialogTitle className="text-white">Salir del wizard</AlertDialogTitle>
                    <AlertDialogDescription className="text-zinc-400">
                        Perderas todo el progreso del wizard. ¿Deseas salir?
                    </AlertDialogDescription>
                </AlertDialogHeader>
                <AlertDialogFooter>
                    <AlertDialogCancel>Seguir editando</AlertDialogCancel>
                    <AlertDialogAction
                        className="bg-red-600 hover:bg-red-700"
                        onClick={onConfirmar}
                    >
                        Salir sin guardar
                    </AlertDialogAction>
                </AlertDialogFooter>
            </AlertDialogContent>
        </AlertDialog>
    )
}
