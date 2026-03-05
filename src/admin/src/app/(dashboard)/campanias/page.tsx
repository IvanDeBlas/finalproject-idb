"use client"

import { useRouter } from "next/navigation"
import { toast } from "sonner"
import { FolderOpen, PlusCircle } from "lucide-react"
import { Button, Card, CardContent, Skeleton } from "@/components/ui"
import {
    useMisCampanias,
    usePublicarCampania,
    useDeleteCampania,
} from "@/hooks/use-campanias"
import { CampaniaListCard } from "./components/list/CampaniaListCard"
import { EmptyState } from "./components/shared/EmptyState"

export default function MisCampaniasPage() {
    const router = useRouter()
    const { data: campanias, isLoading } = useMisCampanias()
    const publishMutation = usePublicarCampania()
    const deleteMutation = useDeleteCampania()

    const handlePublish = async (id: string) => {
        if (window.confirm("Publicar campania ahora? Una vez publicada, sera visible para todos.")) {
            try {
                await publishMutation.mutateAsync(id)
                toast.success("Campania publicada exitosamente")
            } catch {
                toast.error("Error al publicar la campania")
            }
        }
    }

    const handleDelete = async (id: string) => {
        if (window.confirm("Eliminar campania? Esta accion no se puede deshacer.")) {
            try {
                await deleteMutation.mutateAsync(id)
                toast.success("Campania eliminada exitosamente")
            } catch {
                toast.error("Error al eliminar la campania")
            }
        }
    }

    // Loading state
    if (isLoading) {
        return (
            <div className="space-y-6">
                <div className="flex items-center justify-between">
                    <div>
                        <Skeleton className="h-8 w-48" />
                        <Skeleton className="h-4 w-64 mt-2" />
                    </div>
                    <Skeleton className="h-10 w-40" />
                </div>
                <div className="space-y-4">
                    {[1, 2, 3].map((i) => (
                        <Card key={i} className="bg-card border-border">
                            <CardContent className="flex items-center gap-6 p-6">
                                <Skeleton className="w-24 h-24 rounded-lg" />
                                <div className="flex-1 space-y-2">
                                    <Skeleton className="h-6 w-48" />
                                    <Skeleton className="h-4 w-32" />
                                    <Skeleton className="h-2 w-full" />
                                </div>
                            </CardContent>
                        </Card>
                    ))}
                </div>
            </div>
        )
    }

    // Empty state
    if (!campanias?.length) {
        return (
            <div className="space-y-6">
                <div className="flex items-center justify-between">
                    <div>
                        <h1 className="text-3xl font-bold text-foreground">
                            Mis Campanias
                        </h1>
                        <p className="text-muted-foreground">
                            Gestiona tus campanias de crowdfunding
                        </p>
                    </div>
                </div>

                <Card className="bg-card border-border">
                    <CardContent className="py-4">
                        <EmptyState
                            icon={<FolderOpen className="w-16 h-16" />}
                            title="No tienes campanias aun"
                            description="Crea tu primera campania para empezar a recaudar fondos para tu proyecto musical"
                            actionLabel="Crear tu primera campania"
                            onAction={() =>
                                router.push("/campanias/nueva")
                            }
                        />
                    </CardContent>
                </Card>
            </div>
        )
    }

    return (
        <div className="space-y-6">
            {/* Header */}
            <div className="flex items-center justify-between">
                <div>
                    <h1 className="text-3xl font-bold text-foreground">
                        Mis Campanias
                    </h1>
                    <p className="text-muted-foreground">
                        Gestiona tus campanias de crowdfunding
                    </p>
                </div>
                <Button
                    onClick={() => router.push("/campanias/nueva")}
                    className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold"
                >
                    <PlusCircle className="mr-2 h-4 w-4" />
                    Nueva campania
                </Button>
            </div>

            {/* Campaign list */}
            <div className="space-y-4">
                {campanias.map((campania) => (
                    <CampaniaListCard
                        key={campania.id}
                        campania={campania}
                        onEdit={(id) =>
                            router.push(`/campanias/${id}/editar`)
                        }
                        onView={(id) =>
                            router.push(`/campanias/${id}`)
                        }
                        onPublish={handlePublish}
                        onDelete={handleDelete}
                    />
                ))}
            </div>
        </div>
    )
}
