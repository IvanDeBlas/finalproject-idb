"use client"

import { ArtistaForm } from "@/components/artistas/artista-form"
import { useMyArtistProfile, useCreateArtista, useUpdateArtista } from "@/hooks/use-artista"
import { toast } from "sonner"
import type { ArtistaFormData } from "@shared/schemas"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Skeleton } from "@/components/ui/skeleton"

export default function PerfilPage() {
  const { data: artista, isLoading } = useMyArtistProfile()
  const createMutation = useCreateArtista()
  const updateMutation = useUpdateArtista()

  const handleSubmit = async (data: ArtistaFormData) => {
    try {
      if (artista) {
        await updateMutation.mutateAsync({ id: artista.id, data })
        toast.success("Perfil actualizado")
      } else {
        await createMutation.mutateAsync(data)
        toast.success("Perfil creado")
      }
    } catch {
      toast.error("Error al guardar el perfil")
    }
  }

  if (isLoading) {
    return (
      <div className="max-w-2xl mx-auto">
        <Card>
          <CardHeader>
            <Skeleton className="h-8 w-48" />
            <Skeleton className="h-4 w-72" />
          </CardHeader>
          <CardContent className="space-y-4">
            <Skeleton className="h-10 w-full" />
            <Skeleton className="h-10 w-full" />
            <Skeleton className="h-24 w-full" />
          </CardContent>
        </Card>
      </div>
    )
  }

  return (
    <div className="max-w-2xl mx-auto">
      <Card>
        <CardHeader>
          <CardTitle>
            {artista ? "Editar Perfil" : "Crear Perfil de Artista"}
          </CardTitle>
          <CardDescription>
            {artista
              ? "Actualiza la informacion de tu perfil artistico"
              : "Completa tu perfil para poder crear campanias"}
          </CardDescription>
        </CardHeader>
        <CardContent>
          <ArtistaForm
            defaultValues={artista ? {
              nombreArtistico: artista.nombreArtistico,
              descripcion: artista.descripcion || "",
              imagenUrl: artista.imagenUrl || "",
              generoMusical: artista.generoMusical || "",
            } : undefined}
            onSubmit={handleSubmit}
            isSubmitting={createMutation.isPending || updateMutation.isPending}
          />
        </CardContent>
      </Card>
    </div>
  )
}
