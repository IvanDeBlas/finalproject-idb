"use client"

import {
    Table,
    TableBody,
    TableHead,
    TableHeader,
    TableRow,
} from "@/components/ui/table"
import { Card } from "@/components/ui/card"
import { Skeleton } from "@/components/ui/skeleton"
import { TemplateRow } from "./TemplateRow"
import { EmptyState } from "../shared/EmptyState"
import type { PlantillaProyectoList } from "@shared/types"

interface TemplateTableProps {
    templates: PlantillaProyectoList[]
    onEdit: (id: string) => void
    onView: (id: string) => void
    onToggleStatus: (id: string) => void
    isLoading?: boolean
}

function TableSkeleton() {
    return (
        <TableBody>
            {Array.from({ length: 5 }).map((_, i) => (
                <TableRow key={i}>
                    <td className="p-4"><Skeleton className="h-4 w-48" /></td>
                    <td className="p-4"><Skeleton className="h-4 w-12 mx-auto" /></td>
                    <td className="p-4 hidden lg:table-cell"><Skeleton className="h-4 w-32" /></td>
                    <td className="p-4"><Skeleton className="h-6 w-16 mx-auto" /></td>
                    <td className="p-4"><Skeleton className="h-8 w-48 ml-auto" /></td>
                </TableRow>
            ))}
        </TableBody>
    )
}

export function TemplateTable({
    templates,
    onEdit,
    onView,
    onToggleStatus,
    isLoading = false,
}: TemplateTableProps) {
    if (!isLoading && templates.length === 0) {
        return (
            <Card className="p-6">
                <EmptyState
                    icon="📋"
                    title="No hay templates disponibles"
                    description="Crea el primero para comenzar"
                />
            </Card>
        )
    }

    return (
        <Card>
            <Table>
                <TableHeader>
                    <TableRow>
                        <TableHead className="font-semibold">Nombre</TableHead>
                        <TableHead className="font-semibold text-center w-[100px]">
                            Neces.
                        </TableHead>
                        <TableHead className="font-semibold hidden lg:table-cell w-[150px]">
                            Precio
                        </TableHead>
                        <TableHead className="font-semibold text-center w-[120px]">
                            Estado
                        </TableHead>
                        <TableHead className="font-semibold text-right w-[280px]">
                            Acciones
                        </TableHead>
                    </TableRow>
                </TableHeader>
                {isLoading ? (
                    <TableSkeleton />
                ) : (
                    <TableBody>
                        {templates.map((template) => (
                            <TemplateRow
                                key={template.id}
                                template={template}
                                onEdit={onEdit}
                                onView={onView}
                                onToggleStatus={onToggleStatus}
                            />
                        ))}
                    </TableBody>
                )}
            </Table>
        </Card>
    )
}
