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
import { Loader2 } from "lucide-react"

type DialogVariante = "rechazar" | "bloquear" | "dar-de-baja"

interface InscripcionConfirmDialogProps {
    open: boolean
    onOpenChange: (open: boolean) => void
    onConfirm: () => void
    isPending?: boolean
    variante: DialogVariante
    promotorNombre: string
}

const DIALOG_CONFIG: Record<
    DialogVariante,
    {
        titulo: string
        getDescripcion: (nombre: string) => string
        textoConfirmar: string
        textoLoading: string
        esDestructivo: boolean
    }
> = {
    rechazar: {
        titulo: "Rechazar solicitud",
        getDescripcion: (nombre) =>
            `¿Seguro que deseas rechazar la solicitud de ${nombre}? Podra volver a solicitarse en el futuro.`,
        textoConfirmar: "Rechazar",
        textoLoading: "Rechazando...",
        esDestructivo: false,
    },
    bloquear: {
        titulo: "Bloquear promotor",
        getDescripcion: (nombre) =>
            `¿Seguro que deseas bloquear a ${nombre}? No podra volver a solicitar inscripcion en este programa.`,
        textoConfirmar: "Bloquear",
        textoLoading: "Bloqueando...",
        esDestructivo: true,
    },
    "dar-de-baja": {
        titulo: "Dar de baja al promotor",
        getDescripcion: (nombre) =>
            `¿Seguro que deseas dar de baja a ${nombre}? Su codigo referido quedara desactivado.`,
        textoConfirmar: "Dar de baja",
        textoLoading: "Dando de baja...",
        esDestructivo: true,
    },
}

export function InscripcionConfirmDialog({
    open,
    onOpenChange,
    onConfirm,
    isPending = false,
    variante,
    promotorNombre,
}: InscripcionConfirmDialogProps) {
    const config = DIALOG_CONFIG[variante]

    return (
        <AlertDialog open={open} onOpenChange={onOpenChange}>
            <AlertDialogContent className="bg-[#151525] border-[#334155] sm:max-w-md">
                <AlertDialogHeader>
                    <AlertDialogTitle className="text-white font-semibold">
                        {config.titulo}
                    </AlertDialogTitle>
                    <AlertDialogDescription className="text-[#94a3b8]">
                        {config.getDescripcion(promotorNombre)}
                    </AlertDialogDescription>
                </AlertDialogHeader>
                <AlertDialogFooter>
                    <AlertDialogCancel
                        disabled={isPending}
                        className="border-[#334155] text-[#94a3b8] hover:bg-[#1e1e38] hover:text-white"
                    >
                        Cancelar
                    </AlertDialogCancel>
                    <AlertDialogAction
                        onClick={(e) => {
                            e.preventDefault()
                            onConfirm()
                        }}
                        disabled={isPending}
                        className={
                            config.esDestructivo
                                ? "bg-red-900/80 border border-red-800/50 text-red-300 hover:bg-red-900 hover:text-red-200"
                                : "border-[#334155] text-white hover:bg-[#1e1e38]"
                        }
                    >
                        {isPending && <Loader2 className="w-4 h-4 animate-spin mr-2" />}
                        {isPending ? config.textoLoading : config.textoConfirmar}
                    </AlertDialogAction>
                </AlertDialogFooter>
            </AlertDialogContent>
        </AlertDialog>
    )
}
