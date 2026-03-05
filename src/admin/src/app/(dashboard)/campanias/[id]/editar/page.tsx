"use client"

import { useRouter, useParams } from "next/navigation"
import { AlertCircle, Loader2 } from "lucide-react"
import { Skeleton, Card, CardContent } from "@/components/ui"
import { Alert, AlertTitle, AlertDescription } from "@/components/ui/alert"
import { useCampania } from "@/hooks/use-campanias"
import { CAMPANIA_ESTADOS } from "@shared/constants"
import { WizardContainer } from "../../components/wizard/WizardContainer"
import type { CreateCampaniaFormData } from "@shared/schemas"

export default function EditarCampaniaPage() {
    const params = useParams<{ id: string }>()
    const router = useRouter()
    const { data: campania, isLoading } = useCampania(params.id)

    if (isLoading) {
        return (
            <div className="max-w-4xl mx-auto py-8 px-6 space-y-8">
                <div className="text-center space-y-2">
                    <Skeleton className="h-8 w-48 mx-auto" />
                    <Skeleton className="h-4 w-64 mx-auto" />
                </div>
                <div className="flex justify-center">
                    <Loader2 className="h-8 w-8 animate-spin text-muted-foreground" />
                </div>
                <Card className="bg-card border-border">
                    <CardContent className="p-8 space-y-6">
                        <Skeleton className="h-6 w-48" />
                        <Skeleton className="h-10 w-full" />
                        <Skeleton className="h-10 w-full" />
                        <div className="grid grid-cols-2 gap-4">
                            <Skeleton className="h-10" />
                            <Skeleton className="h-10" />
                        </div>
                    </CardContent>
                </Card>
            </div>
        )
    }

    if (!campania) {
        return (
            <div className="max-w-2xl mx-auto py-16 text-center">
                <h2 className="text-xl font-semibold text-foreground mb-2">
                    Campania no encontrada
                </h2>
                <p className="text-muted-foreground mb-4">
                    La campania que buscas no existe o fue eliminada
                </p>
                <button
                    onClick={() => router.push("/campanias")}
                    className="text-primary hover:underline"
                >
                    Volver a mis campanias
                </button>
            </div>
        )
    }

    if (campania.estadoCampaniaId !== CAMPANIA_ESTADOS.BORRADOR) {
        return (
            <div className="max-w-2xl mx-auto py-16">
                <Alert variant="destructive">
                    <AlertCircle className="h-4 w-4" />
                    <AlertTitle>No se puede editar</AlertTitle>
                    <AlertDescription>
                        Solo se pueden editar campanias en estado borrador. Esta
                        campania ya fue publicada.
                    </AlertDescription>
                </Alert>
                <div className="mt-4 text-center">
                    <button
                        onClick={() =>
                            router.push(`/campanias/${params.id}`)
                        }
                        className="text-primary hover:underline"
                    >
                        Ver campania
                    </button>
                </div>
            </div>
        )
    }

    // Map Campania to CreateCampaniaFormData for the wizard
    const initialData: Partial<CreateCampaniaFormData> = {
        titulo: campania.titulo,
        subtitulo: campania.subtitulo || "",
        descripcionCorta: campania.descripcionCorta || "",
        imagenPrincipalUrl: campania.imagenPrincipalUrl || "",
        videoPrincipalUrl: campania.videoPrincipalUrl || "",
        importeObjetivo: campania.importeObjetivo,
        monedaId: campania.monedaId,
        tipoFinanciacionId: campania.tipoFinanciacionId,
        permiteAportacionesAnonimas: campania.permiteAportacionesAnonimas,
        permitePropinas: campania.permitePropinas,
        fechaFin: campania.fechaFin || "",
        fechaInicio: campania.fechaInicio || "",
        proyectoArtisticoId: campania.proyectoArtisticoId || "",
    }

    return (
        <WizardContainer
            mode="edit"
            campaniaId={params.id}
            initialData={initialData}
        />
    )
}
