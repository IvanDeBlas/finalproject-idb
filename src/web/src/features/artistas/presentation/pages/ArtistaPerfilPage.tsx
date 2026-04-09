import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Textarea } from "@/components/ui/textarea"
import {
  useMyArtistProfile,
  useCreateArtista,
  useUpdateArtista,
  artistaSchema,
  type ArtistaFormData,
} from "../../application"

export default function ArtistaPerfilPage() {
  const { data: artista, isLoading } = useMyArtistProfile()
  const createMutation = useCreateArtista()
  const updateMutation = useUpdateArtista()

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<ArtistaFormData>({
    resolver: zodResolver(artistaSchema),
    values: artista
      ? {
          nombreArtistico: artista.nombreArtistico,
          descripcion: artista.descripcion || "",
          imagenUrl: artista.imagenUrl || "",
          generoMusical: artista.generoMusical || "",
        }
      : undefined,
  })

  const onSubmit = async (data: ArtistaFormData) => {
    if (artista) {
      await updateMutation.mutateAsync({ id: artista.id, data })
    } else {
      await createMutation.mutateAsync(data)
    }
  }

  if (isLoading) {
    return (
      <div className="mx-auto max-w-2xl">
        <div className="h-96 animate-pulse rounded-lg bg-muted" />
      </div>
    )
  }

  return (
    <div className="mx-auto max-w-2xl">
      <Card>
        <CardHeader>
          <CardTitle>
            {artista ? "Editar perfil de artista" : "Crear perfil de artista"}
          </CardTitle>
          <CardDescription>
            {artista
              ? "Actualiza la informacion de tu perfil artistico"
              : "Completa tu perfil para poder crear campanias"}
          </CardDescription>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
            <div className="space-y-2">
              <Label htmlFor="nombreArtistico">Nombre artistico</Label>
              <Input
                id="nombreArtistico"
                placeholder="Tu nombre de artista"
                {...register("nombreArtistico")}
              />
              {errors.nombreArtistico && (
                <p className="text-sm text-destructive">
                  {errors.nombreArtistico.message}
                </p>
              )}
            </div>

            <div className="space-y-2">
              <Label htmlFor="generoMusical">Genero musical</Label>
              <Input
                id="generoMusical"
                placeholder="Ej: Rock, Pop, Electronica..."
                {...register("generoMusical")}
              />
              {errors.generoMusical && (
                <p className="text-sm text-destructive">
                  {errors.generoMusical.message}
                </p>
              )}
            </div>

            <div className="space-y-2">
              <Label htmlFor="descripcion">Biografia</Label>
              <Textarea
                id="descripcion"
                placeholder="Cuentanos sobre ti y tu musica..."
                rows={5}
                {...register("descripcion")}
              />
              {errors.descripcion && (
                <p className="text-sm text-destructive">
                  {errors.descripcion.message}
                </p>
              )}
            </div>

            <div className="space-y-2">
              <Label htmlFor="imagenUrl">URL de imagen de perfil</Label>
              <Input
                id="imagenUrl"
                type="url"
                placeholder="https://..."
                {...register("imagenUrl")}
              />
              {errors.imagenUrl && (
                <p className="text-sm text-destructive">
                  {errors.imagenUrl.message}
                </p>
              )}
            </div>

            <Button type="submit" disabled={isSubmitting}>
              {isSubmitting
                ? "Guardando..."
                : artista
                ? "Actualizar perfil"
                : "Crear perfil"}
            </Button>
          </form>
        </CardContent>
      </Card>
    </div>
  )
}
