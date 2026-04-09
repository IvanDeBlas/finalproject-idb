import { useNavigate } from "react-router-dom"
import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Textarea } from "@/components/ui/textarea"
import { ROUTES } from "@/lib/constants"
import { useCreateCampania, createCampaniaSchema, type CreateCampaniaFormData } from "../../application"

export default function CampaniaNewPage() {
  const navigate = useNavigate()
  const createMutation = useCreateCampania()

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<CreateCampaniaFormData>({
    resolver: zodResolver(createCampaniaSchema),
  })

  const onSubmit = async (data: CreateCampaniaFormData) => {
    try {
      await createMutation.mutateAsync(data)
      navigate(ROUTES.DASHBOARD)
    } catch {
      // Error is handled by the mutation's onError callback
    }
  }

  return (
    <div className="mx-auto max-w-2xl">
      <Card>
        <CardHeader>
          <CardTitle>Nueva Campania</CardTitle>
          <CardDescription>
            Crea una nueva campania de crowdfunding para tu proyecto musical
          </CardDescription>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
            <div className="space-y-2">
              <Label htmlFor="titulo">Titulo</Label>
              <Input
                id="titulo"
                placeholder="Nombre de tu campania"
                {...register("titulo")}
              />
              {errors.titulo && (
                <p className="text-sm text-destructive">{errors.titulo.message}</p>
              )}
            </div>

            <div className="space-y-2">
              <Label htmlFor="descripcion">Descripcion</Label>
              <Textarea
                id="descripcion"
                placeholder="Describe tu proyecto musical..."
                rows={5}
                {...register("descripcion")}
              />
              {errors.descripcion && (
                <p className="text-sm text-destructive">{errors.descripcion.message}</p>
              )}
            </div>

            <div className="grid gap-4 sm:grid-cols-2">
              <div className="space-y-2">
                <Label htmlFor="importeObjetivo">Meta financiera (EUR)</Label>
                <Input
                  id="importeObjetivo"
                  type="number"
                  min={100}
                  placeholder="5000"
                  {...register("importeObjetivo", { valueAsNumber: true })}
                />
                {errors.importeObjetivo && (
                  <p className="text-sm text-destructive">
                    {errors.importeObjetivo.message}
                  </p>
                )}
              </div>

              <div className="space-y-2">
                <Label htmlFor="fechaFin">Fecha de finalizacion</Label>
                <Input
                  id="fechaFin"
                  type="date"
                  {...register("fechaFin", { valueAsDate: true })}
                />
                {errors.fechaFin && (
                  <p className="text-sm text-destructive">{errors.fechaFin.message}</p>
                )}
              </div>
            </div>

            <div className="space-y-2">
              <Label htmlFor="imagenUrl">URL de imagen (opcional)</Label>
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

            <div className="flex gap-4">
              <Button type="submit" disabled={isSubmitting}>
                {isSubmitting ? "Creando..." : "Crear campania"}
              </Button>
              <Button
                type="button"
                variant="outline"
                onClick={() => navigate(ROUTES.DASHBOARD)}
              >
                Cancelar
              </Button>
            </div>
          </form>
        </CardContent>
      </Card>
    </div>
  )
}
