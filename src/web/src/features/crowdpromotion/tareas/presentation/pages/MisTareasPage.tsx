import { useState, useMemo, useEffect } from "react"
import { useParams, useNavigate, Link } from "react-router-dom"
import { toast } from "sonner"
import { AlertCircle, ClipboardX, ChevronRight } from "lucide-react"
import { Button } from "@/components/ui/button"
import { Skeleton } from "@/components/ui/skeleton"
import { ESTADO_TAREA_PROMO } from "@shared/constants"
import { puedeCompletarTarea } from "@shared/utils/mappers"
import type { MisTareasItem, CompletarTareaResponse } from "../../domain"
import { useMisTareas } from "../../application/hooks/useMisTareas"
import { useCompletarTarea } from "../../application/hooks/useCompletarTarea"
import { TareaCard } from "../components/TareaCard"
import { CompletarTareaDialog } from "../components/CompletarTareaDialog"
import type { CompletarTareaFormData } from "@shared/schemas/crowdpromotion.schema"

function sortTareasByPriority(items: MisTareasItem[]): MisTareasItem[] {
    return [...items].sort((a, b) => {
        const getPriority = (item: MisTareasItem): number => {
            if (
                puedeCompletarTarea(item) ||
                item.miEstado?.estadoTareaId === ESTADO_TAREA_PROMO.RECHAZADA
            ) {
                return 0
            }
            if (item.miEstado?.estadoTareaId === ESTADO_TAREA_PROMO.COMPLETADA) {
                return 1
            }
            if (item.miEstado?.estadoTareaId === ESTADO_TAREA_PROMO.VALIDADA) {
                return 2
            }
            return 3
        }
        const priorityDiff = getPriority(a) - getPriority(b)
        if (priorityDiff !== 0) return priorityDiff
        return a.orden - b.orden
    })
}

export default function MisTareasPage() {
    const { programaId } = useParams<{ programaId: string }>()
    const navigate = useNavigate()

    const [tareaSeleccionada, setTareaSeleccionada] =
        useState<MisTareasItem | null>(null)
    const [dialogOpen, setDialogOpen] = useState(false)
    const [esReenvio, setEsReenvio] = useState(false)

    const { data, isLoading, isError, error, refetch } = useMisTareas(
        programaId ?? ""
    )
    const completarMutation = useCompletarTarea()

    // Redirect on specific error codes
    useEffect(() => {
        if (!isError || !error) return
        const errorCode = (error as Error & { errorCode?: string }).errorCode
        if (errorCode === "4026") {
            toast.error("No estas aprobado en este programa.")
            navigate("/promotor/mis-programas", { replace: true })
        } else if (errorCode === "4024") {
            toast.error("Este programa no esta activo.")
            navigate("/promotor/mis-programas", { replace: true })
        }
    }, [isError, error, navigate])

    const sortedItems = useMemo(() => {
        if (!data?.items) return []
        return sortTareasByPriority(data.items)
    }, [data?.items])

    const handleCompletar = (tarea: MisTareasItem, reenvio: boolean) => {
        setTareaSeleccionada(tarea)
        setEsReenvio(reenvio)
        setDialogOpen(true)
    }

    const handleDialogSubmit = async (formData: CompletarTareaFormData) => {
        if (!programaId || !tareaSeleccionada) return
        await completarMutation.mutateAsync({
            programaId,
            tareaId: tareaSeleccionada.tareaId,
            data: {
                urlPruebaCompletado: formData.urlPruebaCompletado,
                comentarioPromotor: formData.comentarioPromotor || undefined,
            },
            esReenvio,
        })
        setDialogOpen(false)
        setTareaSeleccionada(null)
    }

    const handleDialogSuccess = (_response: CompletarTareaResponse) => {
        setDialogOpen(false)
        setTareaSeleccionada(null)
    }

    const programaTitulo = data?.programaTitulo ?? ""

    return (
        <div className="min-h-screen bg-[#1a1a2e]">
            <div className="max-w-3xl mx-auto px-4 pt-6 pb-10">
                {/* Breadcrumb */}
                <nav
                    aria-label="Breadcrumb"
                    className="flex items-center gap-1.5 text-xs text-[#64748b] mb-2"
                >
                    <Link
                        to="/promotor/mis-programas"
                        className="hover:text-[#94a3b8] transition-colors"
                    >
                        Mis programas
                    </Link>
                    <ChevronRight
                        className="w-3 h-3 text-[#334155]"
                        aria-hidden="true"
                    />
                    <span className="max-w-[12rem] truncate hover:text-[#94a3b8] cursor-pointer transition-colors">
                        {programaTitulo}
                    </span>
                    <ChevronRight
                        className="w-3 h-3 text-[#334155]"
                        aria-hidden="true"
                    />
                    <span className="text-[#94a3b8]" aria-current="page">
                        Tareas
                    </span>
                </nav>

                {/* Header */}
                <div className="mb-6">
                    <h1 className="text-2xl font-bold text-white leading-tight">
                        Tareas: {programaTitulo}
                    </h1>
                    <p className="text-sm text-[#94a3b8] mt-1">
                        Completa las tareas y acumula recompensas
                    </p>
                </div>

                {/* Loading */}
                {isLoading && (
                    <div
                        className="flex flex-col gap-4"
                        aria-busy="true"
                        aria-label="Cargando tareas"
                    >
                        {Array.from({ length: 3 }).map((_, i) => (
                            <Skeleton
                                key={i}
                                className="bg-[#1e1e38] rounded-xl h-[180px]"
                            />
                        ))}
                    </div>
                )}

                {/* Error */}
                {isError && !isLoading && (
                    <div
                        className="bg-[#1e1e38] rounded-xl p-10 flex flex-col items-center gap-4 text-center"
                        role="alert"
                    >
                        <AlertCircle
                            className="w-8 h-8 text-red-400"
                            aria-hidden="true"
                        />
                        <p className="text-sm text-[#94a3b8]">
                            No se pudieron cargar las tareas
                        </p>
                        <Button
                            variant="outline"
                            size="sm"
                            className="border-[#334155] text-[#94a3b8] hover:text-white hover:bg-[#1e1e38] mt-2"
                            onClick={() => refetch()}
                        >
                            Reintentar
                        </Button>
                    </div>
                )}

                {/* Empty state */}
                {!isLoading && !isError && data && data.items.length === 0 && (
                    <div className="bg-[#1e1e38] rounded-xl p-10 flex flex-col items-center gap-4 text-center">
                        <ClipboardX
                            className="w-12 h-12 text-[#64748b]"
                            aria-hidden="true"
                        />
                        <p className="text-base font-semibold text-[#94a3b8]">
                            Este programa no tiene tareas activas
                        </p>
                        <p className="text-sm text-[#64748b]">
                            El artista aun no ha configurado tareas de promocion
                        </p>
                    </div>
                )}

                {/* Task list */}
                {!isLoading && !isError && data && data.items.length > 0 && (
                    <div className="flex flex-col gap-4" role="list">
                        {sortedItems.map((tarea) => (
                            <TareaCard
                                key={tarea.tareaId}
                                tarea={tarea}
                                onCompletar={handleCompletar}
                                isProgramaActivo={true}
                                isSubmitting={
                                    completarMutation.isPending &&
                                    completarMutation.variables?.tareaId ===
                                        tarea.tareaId
                                }
                            />
                        ))}
                    </div>
                )}
            </div>

            {/* Complete dialog */}
            <CompletarTareaDialog
                open={dialogOpen}
                onOpenChange={setDialogOpen}
                tarea={tareaSeleccionada}
                programaId={programaId ?? ""}
                esReenvio={esReenvio}
                onSubmit={handleDialogSubmit}
                isSubmitting={completarMutation.isPending}
            />
        </div>
    )
}
