"use client"

import { useState } from "react"
import { Button } from "@/components/ui/button"
import { Skeleton } from "@/components/ui/skeleton"
import { CheckCircle, AlertCircle, ChevronLeft, ChevronRight } from "lucide-react"
import { useTareasPendientes } from "@/hooks/use-tareas-pendientes"
import { TareaPendienteCard } from "./TareaPendienteCard"
import { ValidarTareaDialog } from "./ValidarTareaDialog"
import { RechazarTareaDialog } from "./RechazarTareaDialog"
import type { TareaPendienteItem } from "@shared/types/crowdpromotion"

interface PromoProgramaPendientesTabProps {
    programaId: string
}

export function PromoProgramaPendientesTab({ programaId }: PromoProgramaPendientesTabProps) {
    const [selectedItem, setSelectedItem] = useState<TareaPendienteItem | null>(null)
    const [dialogMode, setDialogMode] = useState<"validar" | "rechazar" | null>(null)
    const [page, setPage] = useState(1)

    const { data, isLoading, isError, refetch } = useTareasPendientes(programaId, { page })

    const handleValidar = (item: TareaPendienteItem) => {
        setSelectedItem(item)
        setDialogMode("validar")
    }

    const handleRechazar = (item: TareaPendienteItem) => {
        setSelectedItem(item)
        setDialogMode("rechazar")
    }

    const handleClose = () => {
        setSelectedItem(null)
        setDialogMode(null)
    }

    if (isLoading) {
        return (
            <div className="space-y-4 mt-4" aria-busy="true" aria-label="Cargando tareas pendientes">
                {[1, 2, 3].map((i) => (
                    <Skeleton key={i} className="h-[120px] rounded-xl bg-[#1e1e38]" />
                ))}
            </div>
        )
    }

    if (isError) {
        return (
            <div className="flex flex-col items-center justify-center py-12 text-center mt-4">
                <AlertCircle className="w-10 h-10 text-red-400 mb-3" aria-hidden="true" />
                <p className="text-sm text-red-400 mb-4">Error al cargar las tareas pendientes</p>
                <Button variant="outline" onClick={() => refetch()}>
                    Reintentar
                </Button>
            </div>
        )
    }

    const items = data?.items ?? []
    const totalCount = data?.totalCount ?? 0
    const totalPages = data?.totalPages ?? 0
    const pageSize = data?.pageSize ?? 10

    if (items.length === 0) {
        return (
            <div className="flex flex-col items-center justify-center py-12 text-center mt-4">
                <CheckCircle className="w-10 h-10 text-green-400 mb-3" aria-hidden="true" />
                <p className="text-sm text-[#94a3b8]">No hay tareas pendientes de validacion</p>
                <p className="text-xs text-[#64748b] mt-1">Todas las tareas han sido revisadas</p>
            </div>
        )
    }

    return (
        <div className="mt-4">
            <div className="mb-4">
                <h3 className="text-base font-semibold text-white">
                    Tareas pendientes de validacion ({totalCount})
                </h3>
                <p className="text-sm text-[#94a3b8] mt-0.5">
                    Revisa las pruebas enviadas por los promotores
                </p>
            </div>

            {items.map((item) => (
                <TareaPendienteCard
                    key={item.tareaPromotorId}
                    item={item}
                    onValidar={handleValidar}
                    onRechazar={handleRechazar}
                />
            ))}

            {totalPages > 1 && (
                <div className="flex items-center justify-between mt-6">
                    <p className="text-sm text-[#64748b]">
                        Mostrando {Math.min(pageSize, items.length)} de {totalCount} pendientes
                    </p>
                    <div className="flex items-center gap-2">
                        <Button
                            variant="outline"
                            size="sm"
                            disabled={page === 1}
                            onClick={() => setPage((p) => Math.max(1, p - 1))}
                            className="h-8 px-3 text-xs"
                        >
                            <ChevronLeft className="w-4 h-4 mr-1" aria-hidden="true" />
                            Anterior
                        </Button>
                        <span className="text-sm text-[#94a3b8]">
                            Pagina {page} de {totalPages}
                        </span>
                        <Button
                            variant="outline"
                            size="sm"
                            disabled={page === totalPages}
                            onClick={() => setPage((p) => p + 1)}
                            className="h-8 px-3 text-xs"
                        >
                            Siguiente
                            <ChevronRight className="w-4 h-4 ml-1" aria-hidden="true" />
                        </Button>
                    </div>
                </div>
            )}

            <ValidarTareaDialog
                isOpen={dialogMode === "validar"}
                onClose={handleClose}
                item={selectedItem}
                programaId={programaId}
            />
            <RechazarTareaDialog
                isOpen={dialogMode === "rechazar"}
                onClose={handleClose}
                item={selectedItem}
                programaId={programaId}
            />
        </div>
    )
}
