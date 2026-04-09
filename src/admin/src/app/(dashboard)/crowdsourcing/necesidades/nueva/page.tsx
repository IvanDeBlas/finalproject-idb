"use client"

import { useRouter } from "next/navigation"
import { ArrowLeft } from "lucide-react"
import { Button } from "@/components/ui/button"
import { useCreateNecesidad } from "@/hooks/use-necesidades-mutations"
import { NecesidadForm } from "../components/form/NecesidadForm"
import type { CreateNecesidadFormData, UpdateNecesidadFormData } from "@shared/schemas/crowdsourcing.schema"
import type { CreateNecesidadRequest } from "@shared/types"

export default function NuevaNecesidadPage() {
    const router = useRouter()
    const createMutation = useCreateNecesidad()

    const handleSubmit = async (data: CreateNecesidadFormData | UpdateNecesidadFormData) => {
        await createMutation.mutateAsync(data as CreateNecesidadRequest)
    }

    return (
        <div className="space-y-6">
            <Button
                variant="ghost"
                onClick={() => router.back()}
                className="flex items-center gap-2"
            >
                <ArrowLeft className="h-4 w-4" />
                Volver a Mis Necesidades
            </Button>

            <div>
                <h1 className="text-3xl font-bold">Publicar Nueva Necesidad</h1>
                <p className="text-muted-foreground mt-1">
                    Completa los detalles de tu necesidad para recibir propuestas
                </p>
            </div>

            <NecesidadForm
                mode="create"
                onSubmit={handleSubmit}
                onCancel={() => router.back()}
            />
        </div>
    )
}
