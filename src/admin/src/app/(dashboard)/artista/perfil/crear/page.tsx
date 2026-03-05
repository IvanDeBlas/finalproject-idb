"use client"

import { useRouter } from "next/navigation"
import { toast } from "sonner"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { CreateArtistaForm } from "@/components/artistas/CreateArtistaForm"
import { useCreateArtista } from "@/hooks"
import type { CreateArtistaFormData } from "@shared/schemas"

export default function CrearPerfilArtistaPage() {
  const router = useRouter()
  const createArtistaMutation = useCreateArtista()

  const handleSubmit = async (data: CreateArtistaFormData) => {
    try {
      await createArtistaMutation.mutateAsync({
        nombreArtistico: data.nombreArtistico,
        descripcion: data.descripcion || undefined,
        pais: data.pais || undefined,
        ciudad: data.ciudad || undefined,
        imagenUrl: data.imagenUrl || undefined,
      })

      toast.success("Perfil de artista creado exitosamente!")
      router.push("/dashboard")
    } catch {
      toast.error("Error al crear el perfil. Intenta nuevamente.")
    }
  }

  return (
    <div className="container max-w-2xl py-8">
      <Card className="bg-gray-800 border-gray-700">
        <CardHeader>
          <CardTitle className="text-white text-2xl">Crear Perfil de Artista</CardTitle>
          <CardDescription className="text-gray-400">
            Completa tu perfil para empezar a crear campañas de crowdfunding
          </CardDescription>
        </CardHeader>
        <CardContent>
          <CreateArtistaForm
            onSubmit={handleSubmit}
            isSubmitting={createArtistaMutation.isPending}
          />
        </CardContent>
      </Card>
    </div>
  )
}
