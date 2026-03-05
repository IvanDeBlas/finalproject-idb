"use client"

import { useState } from "react"
import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Textarea } from "@/components/ui/textarea"
import { createArtistaSchema, type CreateArtistaFormData } from "@shared/schemas"

interface CreateArtistaFormProps {
  onSubmit: (data: CreateArtistaFormData) => Promise<void>
  isSubmitting?: boolean
}

export function CreateArtistaForm({ onSubmit, isSubmitting }: CreateArtistaFormProps) {
  const [descripcionLength, setDescripcionLength] = useState(0)

  const {
    register,
    handleSubmit,
    formState: { errors },
    watch,
  } = useForm<CreateArtistaFormData>({
    resolver: zodResolver(createArtistaSchema),
  })

  // Watch descripcion field for character count
  const descripcion = watch("descripcion", "")

  // Update character count when descripcion changes
  useState(() => {
    setDescripcionLength(descripcion?.length || 0)
  })

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
      <div className="space-y-2">
        <Label htmlFor="nombreArtistico" className="text-white">
          Nombre artístico *
        </Label>
        <Input
          id="nombreArtistico"
          placeholder="Tu nombre de artista"
          className="bg-gray-900 border-gray-700 text-white placeholder:text-gray-500"
          {...register("nombreArtistico")}
        />
        {errors.nombreArtistico && (
          <p className="text-sm text-destructive">{errors.nombreArtistico.message}</p>
        )}
      </div>

      <div className="space-y-2">
        <div className="flex items-center justify-between">
          <Label htmlFor="descripcion" className="text-white">
            Descripción
          </Label>
          <span className="text-xs text-gray-400">
            {descripcionLength} / 2000
          </span>
        </div>
        <Textarea
          id="descripcion"
          placeholder="Cuéntanos sobre ti y tu música..."
          rows={5}
          className="bg-gray-900 border-gray-700 text-white placeholder:text-gray-500 resize-none"
          maxLength={2000}
          {...register("descripcion")}
          onChange={(e) => {
            register("descripcion").onChange(e)
            setDescripcionLength(e.target.value.length)
          }}
        />
        {errors.descripcion && (
          <p className="text-sm text-destructive">{errors.descripcion.message}</p>
        )}
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div className="space-y-2">
          <Label htmlFor="pais" className="text-white">
            País
          </Label>
          <Input
            id="pais"
            placeholder="Ej: España"
            className="bg-gray-900 border-gray-700 text-white placeholder:text-gray-500"
            {...register("pais")}
          />
          {errors.pais && (
            <p className="text-sm text-destructive">{errors.pais.message}</p>
          )}
        </div>

        <div className="space-y-2">
          <Label htmlFor="ciudad" className="text-white">
            Ciudad
          </Label>
          <Input
            id="ciudad"
            placeholder="Ej: Madrid"
            className="bg-gray-900 border-gray-700 text-white placeholder:text-gray-500"
            {...register("ciudad")}
          />
          {errors.ciudad && (
            <p className="text-sm text-destructive">{errors.ciudad.message}</p>
          )}
        </div>
      </div>

      <div className="space-y-2">
        <Label htmlFor="imagenUrl" className="text-white">
          URL de imagen de perfil
        </Label>
        <Input
          id="imagenUrl"
          type="url"
          placeholder="https://ejemplo.com/imagen.jpg"
          className="bg-gray-900 border-gray-700 text-white placeholder:text-gray-500"
          {...register("imagenUrl")}
        />
        {errors.imagenUrl && (
          <p className="text-sm text-destructive">{errors.imagenUrl.message}</p>
        )}
        <p className="text-xs text-gray-400">
          Ingresa una URL válida de tu foto de perfil
        </p>
      </div>

      <Button
        type="submit"
        className="w-full bg-primary hover:bg-primary/90"
        disabled={isSubmitting}
      >
        {isSubmitting ? "Creando perfil..." : "Crear perfil de artista"}
      </Button>
    </form>
  )
}
