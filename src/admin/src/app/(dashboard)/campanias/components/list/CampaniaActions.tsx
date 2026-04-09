"use client"

import { MoreVertical, Trash2, Eye, Pencil, Send } from "lucide-react"
import { Button } from "@/components/ui"
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuSeparator,
    DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu"
import { CAMPANIA_ESTADOS } from "@shared/constants"

interface CampaniaActionsProps {
    campaniaId: string
    estado: number
    onEdit: (id: string) => void
    onView: (id: string) => void
    onPublish: (id: string) => void
    onDelete: (id: string) => void
}

export function CampaniaActions({
    campaniaId,
    estado,
    onEdit,
    onView,
    onPublish,
    onDelete,
}: CampaniaActionsProps) {
    const isBorrador = estado === CAMPANIA_ESTADOS.BORRADOR

    return (
        <div
            className="flex items-center gap-2"
            onClick={(e) => e.stopPropagation()}
        >
            {isBorrador && (
                <>
                    <Button
                        variant="outline"
                        size="sm"
                        onClick={() => onEdit(campaniaId)}
                        className="hidden sm:inline-flex"
                    >
                        <Pencil className="mr-1 h-3.5 w-3.5" />
                        Editar
                    </Button>
                    <Button
                        variant="outline"
                        size="sm"
                        onClick={() => onPublish(campaniaId)}
                        className="hidden sm:inline-flex border-green-500/50 text-green-400 hover:bg-green-500/10"
                    >
                        <Send className="mr-1 h-3.5 w-3.5" />
                        Publicar
                    </Button>
                </>
            )}
            <Button
                variant="outline"
                size="sm"
                onClick={() => onView(campaniaId)}
                className="hidden sm:inline-flex"
            >
                <Eye className="mr-1 h-3.5 w-3.5" />
                Ver
            </Button>

            <DropdownMenu>
                <DropdownMenuTrigger asChild>
                    <Button variant="ghost" size="icon" className="h-8 w-8">
                        <MoreVertical className="h-4 w-4" />
                    </Button>
                </DropdownMenuTrigger>
                <DropdownMenuContent align="end" className="bg-popover border-border">
                    <DropdownMenuItem onClick={() => onView(campaniaId)}>
                        <Eye className="mr-2 h-4 w-4" />
                        Ver detalles
                    </DropdownMenuItem>
                    {isBorrador && (
                        <>
                            <DropdownMenuItem
                                onClick={() => onEdit(campaniaId)}
                            >
                                <Pencil className="mr-2 h-4 w-4" />
                                Editar
                            </DropdownMenuItem>
                            <DropdownMenuItem
                                onClick={() => onPublish(campaniaId)}
                            >
                                <Send className="mr-2 h-4 w-4" />
                                Publicar
                            </DropdownMenuItem>
                        </>
                    )}
                    <DropdownMenuSeparator />
                    <DropdownMenuItem
                        onClick={() => onDelete(campaniaId)}
                        className="text-destructive focus:text-destructive"
                    >
                        <Trash2 className="mr-2 h-4 w-4" />
                        Eliminar
                    </DropdownMenuItem>
                </DropdownMenuContent>
            </DropdownMenu>
        </div>
    )
}
