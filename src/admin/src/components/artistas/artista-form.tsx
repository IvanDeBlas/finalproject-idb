"use client"

import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Textarea } from "@/components/ui/textarea"
import { artistaSchema, type ArtistaFormData } from "@shared/schemas"
import { GENEROS_MUSICALES } from "@shared/constants"

interface ArtistaFormProps {
  defaultValues?: Partial<ArtistaFormData>
  onSubmit: (data: ArtistaFormData) => Promise<void>
  isSubmitting?: boolean
}

export function ArtistaForm({ defaultValues, onSubmit, isSubmitting }: ArtistaFormProps) {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<ArtistaFormData>({
    resolver: zodResolver(artistaSchema),
    defaultValues,
  })

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
      <div className="space-y-2">
        <Label htmlFor="nombreArtistico">Nombre artistico</Label>
        <Input
          id="nombreArtistico"
          placeholder="Tu nombre de artista"
          {...register("nombreArtistico")}
        />
        {errors.nombreArtistico && (
          <p className="text-sm text-destructive">{errors.nombreArtistico.message}</p>
        )}
      </div>

      <div className="space-y-2">
        <Label htmlFor="generoMusical">Genero musical</Label>
        <select
          id="generoMusical"
          className="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2"
          {...register("generoMusical")}
        >
          <option value="">Seleccionar genero</option>
          {GENEROS_MUSICALES.map((genero) => (
            <option key={genero} value={genero}>
              {genero}
            </option>
          ))}
        </select>
        {errors.generoMusical && (
          <p className="text-sm text-destructive">{errors.generoMusical.message}</p>
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
          <p className="text-sm text-destructive">{errors.descripcion.message}</p>
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
          <p className="text-sm text-destructive">{errors.imagenUrl.message}</p>
        )}
      </div>

      <Button type="submit" disabled={isSubmitting}>
        {isSubmitting ? "Guardando..." : defaultValues ? "Actualizar perfil" : "Crear perfil"}
      </Button>
    </form>
  )
}
