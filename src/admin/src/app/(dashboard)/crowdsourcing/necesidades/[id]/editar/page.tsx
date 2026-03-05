"use client"

import { useEffect } from "react"
import { useRouter, useParams } from "next/navigation"
import { ArrowLeft } from "lucide-react"
import { Button } from "@/components/ui/button"
import { Skeleton } from "@/components/ui/skeleton"
import { toast } from "sonner"
import { useNecesidad } from "@/hooks/use-necesidades"
import { useUpdateNecesidad } from "@/hooks/use-necesidades-mutations"
import { NecesidadForm } from "../../components/form/NecesidadForm"
import { APP_ROUTES, ESTADO_NECESIDAD } from "@shared/constants"
import type { CreateNecesidadFormData, UpdateNecesidadFormData } from "@shared/schemas/crowdsourcing.schema"

export default function EditarNecesidadPage() {
    const router = useRouter()
    const params = useParams<{ id: string }>()
    const id = params.id

    const { data: necesidad, isLoading } = useNecesidad(id)
    const updateMutation = useUpdateNecesidad(id)

    useEffect(() => {
        if (necesidad && necesidad.estadoNecesidadId !== ESTADO_NECESIDAD.ABIERTA) {
            toast.error("Solo se pueden editar necesidades en estado Abierta")
            router.push(APP_ROUTES.dashboard.crowdsourcing.necesidadDetail(id))
        }
    }, [necesidad, id, router])

    const handleSubmit = async (data: CreateNecesidadFormData | UpdateNecesidadFormData) => {
        await updateMutation.mutateAsync(data as Parameters<typeof updateMutation.mutateAsync>[0])
    }

    if (isLoading) {
        return (
            <div className="space-y-6">
                <Skeleton className="h-10 w-48" />
                <Skeleton className="h-8 w-64" />
                <Skeleton className="h-96" />
            </div>
        )
    }

    if (!necesidad) {
        return (
            <div className="text-center py-12">
                <p className="text-muted-foreground">Necesidad no encontrada</p>
                <Button
                    variant="outline"
                    className="mt-4"
                    onClick={() =>
                        router.push(APP_ROUTES.dashboard.crowdsourcing.necesidades)
                    }
                >
                    Volver a Mis Necesidades
                </Button>
            </div>
        )
    }

    return (
        <div className="space-y-6">
            <Button
                variant="ghost"
                onClick={() => router.back()}
                className="flex items-center gap-2"
            >
                <ArrowLeft className="h-4 w-4" />
                Volver
            </Button>

            <div>
                <h1 className="text-3xl font-bold">Editar Necesidad</h1>
                <p className="text-muted-foreground mt-1">
                    Modifica los detalles de tu necesidad
                </p>
            </div>

            <NecesidadForm
                mode="edit"
                necesidadId={id}
                defaultValues={{
                    titulo: necesidad.titulo,
                    descripcion: necesidad.descripcion,
                    modalidadTrabajoId: necesidad.modalidadTrabajoId,
                    presupuestoMin: necesidad.presupuestoMin,
                    presupuestoMax: necesidad.presupuestoMax,
                    monedaId: necesidad.monedaId,
                    ubicacionCiudad: necesidad.ubicacionCiudad,
                    ubicacionPais: necesidad.ubicacionPais,
                    fechaLimitePropuestas: necesidad.fechaLimitePropuestas,
                    fechaInicioPrevista: necesidad.fechaInicioPrevista,
                }}
                numeroPropuestas={necesidad.propuestas.length}
                onSubmit={handleSubmit}
                onCancel={() => router.back()}
            />
        </div>
    )
}
