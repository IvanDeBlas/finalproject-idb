import { FC, useEffect } from "react"
import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import {
    Dialog,
    DialogContent,
    DialogHeader,
    DialogTitle,
    DialogFooter,
} from "@/components/ui/dialog"
import { Input } from "@/components/ui/input"
import { Textarea } from "@/components/ui/textarea"
import { Label } from "@/components/ui/label"
import { Button } from "@/components/ui/button"
import { Loader2 } from "lucide-react"
import { createMilestoneSchema } from "@shared/schemas/crowdsourcing.schema"
import type { CreateMilestoneFormData } from "@shared/schemas/crowdsourcing.schema"
import { ImporteAsignadoBar } from "./ImporteAsignadoBar"
import { useCreateMilestone } from "../../application/hooks/useCreateMilestone"
import { useUpdateMilestone } from "../../application/hooks/useUpdateMilestone"
import type { Milestone } from "../../domain"

interface MilestoneFormDialogProps {
    acuerdoId: string
    importeTotal: number
    importeYaAsignado: number
    monedaNombre: string
    fechaInicioAcuerdo: string
    milestoneToEdit?: Milestone | null
    isOpen: boolean
    onClose: () => void
}

export const MilestoneFormDialog: FC<MilestoneFormDialogProps> = ({
    acuerdoId,
    importeTotal,
    importeYaAsignado,
    monedaNombre,
    milestoneToEdit,
    isOpen,
    onClose,
}) => {
    const isEditing = !!milestoneToEdit

    const createMutation = useCreateMilestone(acuerdoId, onClose)
    const updateMutation = useUpdateMilestone(
        acuerdoId,
        milestoneToEdit?.id ?? "",
        onClose
    )

    const isPending = createMutation.isPending || updateMutation.isPending

    const {
        register,
        handleSubmit,
        reset,
        watch,
        formState: { errors },
    } = useForm<CreateMilestoneFormData>({
        resolver: zodResolver(createMilestoneSchema),
        defaultValues: {
            titulo: "",
            descripcion: "",
            importeParcial: 0,
            fechaLimite: "",
        },
    })

    useEffect(() => {
        if (isOpen) {
            if (milestoneToEdit) {
                reset({
                    titulo: milestoneToEdit.titulo,
                    descripcion: milestoneToEdit.descripcion ?? "",
                    importeParcial: milestoneToEdit.importeParcial,
                    fechaLimite: milestoneToEdit.fechaLimite
                        ? milestoneToEdit.fechaLimite.split("T")[0]
                        : "",
                })
            } else {
                reset({
                    titulo: "",
                    descripcion: "",
                    importeParcial: 0,
                    fechaLimite: "",
                })
            }
        }
    }, [isOpen, milestoneToEdit, reset])

    const watchedImporte = watch("importeParcial")
    const importeBaseAjustado = isEditing
        ? importeYaAsignado - (milestoneToEdit?.importeParcial ?? 0)
        : importeYaAsignado
    const importeNuevo = watchedImporte || 0

    const onSubmit = (data: CreateMilestoneFormData) => {
        const payload = {
            titulo: data.titulo,
            descripcion: data.descripcion || undefined,
            importeParcial: data.importeParcial,
            fechaLimite: data.fechaLimite || undefined,
        }

        if (isEditing) {
            updateMutation.mutate(payload)
        } else {
            createMutation.mutate(payload)
        }
    }

    return (
        <Dialog open={isOpen} onOpenChange={(open) => !open && onClose()}>
            <DialogContent className="max-w-md bg-[#0f1729] border-[#334155] text-white">
                <DialogHeader>
                    <DialogTitle className="text-lg font-semibold text-white">
                        {isEditing ? "Editar milestone" : "Agregar milestone"}
                    </DialogTitle>
                </DialogHeader>

                <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
                    <div className="space-y-2">
                        <Label htmlFor="titulo" className="text-slate-300">
                            Titulo
                        </Label>
                        <Input
                            id="titulo"
                            placeholder="Nombre del milestone"
                            className="bg-slate-800 border-slate-600 text-white"
                            {...register("titulo")}
                        />
                        {errors.titulo && (
                            <p className="text-xs text-red-400">
                                {errors.titulo.message}
                            </p>
                        )}
                    </div>

                    <div className="space-y-2">
                        <Label htmlFor="descripcion" className="text-slate-300">
                            Descripcion (opcional)
                        </Label>
                        <Textarea
                            id="descripcion"
                            placeholder="Descripcion del milestone"
                            className="bg-slate-800 border-slate-600 text-white resize-none"
                            rows={3}
                            {...register("descripcion")}
                        />
                        {errors.descripcion && (
                            <p className="text-xs text-red-400">
                                {errors.descripcion.message}
                            </p>
                        )}
                    </div>

                    <div className="space-y-2">
                        <Label htmlFor="importeParcial" className="text-slate-300">
                            Importe ({monedaNombre})
                        </Label>
                        <Input
                            id="importeParcial"
                            type="number"
                            step="0.01"
                            min="0"
                            placeholder="0.00"
                            className="bg-slate-800 border-slate-600 text-white"
                            {...register("importeParcial", { valueAsNumber: true })}
                        />
                        {errors.importeParcial && (
                            <p className="text-xs text-red-400">
                                {errors.importeParcial.message}
                            </p>
                        )}
                    </div>

                    <ImporteAsignadoBar
                        importeAsignado={importeBaseAjustado}
                        importeTotal={importeTotal}
                        porcentaje={0}
                        monedaNombre={monedaNombre}
                        importeNuevo={importeNuevo}
                    />

                    <div className="space-y-2">
                        <Label htmlFor="fechaLimite" className="text-slate-300">
                            Fecha limite (opcional)
                        </Label>
                        <Input
                            id="fechaLimite"
                            type="date"
                            className="bg-slate-800 border-slate-600 text-white"
                            {...register("fechaLimite")}
                        />
                        {errors.fechaLimite && (
                            <p className="text-xs text-red-400">
                                {errors.fechaLimite.message}
                            </p>
                        )}
                    </div>

                    <DialogFooter className="pt-2">
                        <Button
                            type="button"
                            variant="outline"
                            disabled={isPending}
                            className="border-slate-600 text-white hover:bg-slate-800"
                            onClick={onClose}
                        >
                            Cancelar
                        </Button>
                        <Button
                            type="submit"
                            disabled={
                                isPending ||
                                (importeBaseAjustado + importeNuevo > importeTotal)
                            }
                            className="bg-blue-600 hover:bg-blue-700"
                        >
                            {isPending ? (
                                <>
                                    <Loader2 className="w-4 h-4 mr-2 animate-spin" />
                                    Guardando...
                                </>
                            ) : isEditing ? (
                                "Guardar cambios"
                            ) : (
                                "Crear milestone"
                            )}
                        </Button>
                    </DialogFooter>
                </form>
            </DialogContent>
        </Dialog>
    )
}
