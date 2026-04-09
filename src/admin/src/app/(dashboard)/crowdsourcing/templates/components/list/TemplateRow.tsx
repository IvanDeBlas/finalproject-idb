"use client"

import { TableRow, TableCell } from "@/components/ui/table"
import { Button } from "@/components/ui/button"
import { Badge } from "@/components/ui/badge"
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu"
import { MoreVertical, Edit, Eye, ToggleLeft, ToggleRight } from "lucide-react"
import { formatCurrency } from "@/lib/utils"
import type { PlantillaProyectoList } from "@shared/types"

interface TemplateRowProps {
    template: PlantillaProyectoList
    onEdit: (id: string) => void
    onView: (id: string) => void
    onToggleStatus: (id: string) => void
}

export function TemplateRow({
    template,
    onEdit,
    onView,
    onToggleStatus,
}: TemplateRowProps) {
    const isActive = template.orden >= 0

    return (
        <TableRow className="hover:bg-muted/50 transition-colors">
            <TableCell className="font-medium">{template.nombre}</TableCell>
            <TableCell className="text-center text-muted-foreground">
                {template.cantidadNecesidades}
            </TableCell>
            <TableCell className="text-muted-foreground hidden lg:table-cell">
                {formatCurrency(template.precioMinTotal)} - {formatCurrency(template.precioMaxTotal)}
            </TableCell>
            <TableCell className="text-center">
                <Badge variant={isActive ? "default" : "secondary"}>
                    {isActive ? "Activo" : "Inactivo"}
                </Badge>
            </TableCell>
            <TableCell className="text-right">
                <div className="hidden md:flex gap-1 justify-end">
                    <Button
                        variant="ghost"
                        size="sm"
                        onClick={() => onEdit(template.id)}
                        aria-label={`Editar template ${template.nombre}`}
                    >
                        <Edit className="h-4 w-4 mr-1" />
                        Editar
                    </Button>
                    <Button
                        variant="ghost"
                        size="sm"
                        onClick={() => onView(template.id)}
                        aria-label={`Ver template ${template.nombre}`}
                    >
                        <Eye className="h-4 w-4 mr-1" />
                        Ver
                    </Button>
                    <Button
                        variant="ghost"
                        size="sm"
                        onClick={() => onToggleStatus(template.id)}
                        aria-label={`${isActive ? "Desactivar" : "Activar"} template ${template.nombre}`}
                    >
                        {isActive ? (
                            <ToggleRight className="h-4 w-4 mr-1" />
                        ) : (
                            <ToggleLeft className="h-4 w-4 mr-1" />
                        )}
                        {isActive ? "Desactivar" : "Activar"}
                    </Button>
                </div>
                <div className="md:hidden">
                    <DropdownMenu>
                        <DropdownMenuTrigger asChild>
                            <Button variant="ghost" size="sm">
                                <MoreVertical className="h-4 w-4" />
                            </Button>
                        </DropdownMenuTrigger>
                        <DropdownMenuContent align="end">
                            <DropdownMenuItem onClick={() => onEdit(template.id)}>
                                <Edit className="h-4 w-4 mr-2" />
                                Editar
                            </DropdownMenuItem>
                            <DropdownMenuItem onClick={() => onView(template.id)}>
                                <Eye className="h-4 w-4 mr-2" />
                                Ver detalle
                            </DropdownMenuItem>
                            <DropdownMenuItem onClick={() => onToggleStatus(template.id)}>
                                {isActive ? (
                                    <ToggleRight className="h-4 w-4 mr-2" />
                                ) : (
                                    <ToggleLeft className="h-4 w-4 mr-2" />
                                )}
                                {isActive ? "Desactivar" : "Activar"}
                            </DropdownMenuItem>
                        </DropdownMenuContent>
                    </DropdownMenu>
                </div>
            </TableCell>
        </TableRow>
    )
}
