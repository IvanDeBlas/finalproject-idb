import { FC, useEffect } from "react"
import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import {
    Dialog,
    DialogContent,
    DialogHeader,
    DialogTitle,
    DialogDescription,
    DialogFooter,
} from "@/components/ui/dialog"
import { Input } from "@/components/ui/input"
import { Textarea } from "@/components/ui/textarea"
import { Label } from "@/components/ui/label"
import { Button } from "@/components/ui/button"
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from "@/components/ui/select"
import { Upload, Loader2, Info } from "lucide-react"
import { createEntregableSchema } from "@shared/schemas/crowdsourcing.schema"
import type { CreateEntregableFormData } from "@shared/schemas/crowdsourcing.schema"
import { useCreateEntregable } from "../../application/hooks/useCreateEntregable"
import type { Milestone } from "../../domain"

interface SubirEntregableDialogProps {
    acuerdoId: string
    milestones: Milestone[]
    preselectedMilestoneId?: string
    isOpen: boolean
    onClose: () => void
}

export const SubirEntregableDialog: FC<SubirEntregableDialogProps> = ({
    acuerdoId,
    milestones,
    preselectedMilestoneId,
    isOpen,
    onClose,
}) => {
    const mutation = useCreateEntregable(acuerdoId, onClose)

    const {
        register,
        handleSubmit,
        reset,
        setValue,
        formState: { errors },
    } = useForm<CreateEntregableFormData>({
        resolver: zodResolver(createEntregableSchema),
        defaultValues: {
            titulo: "",
            descripcion: "",
            urlRecurso: "",
            milestoneId: undefined,
        },
    })

    useEffect(() => {
        if (isOpen) {
            reset({
                titulo: "",
                descripcion: "",
                urlRecurso: "",
                milestoneId: preselectedMilestoneId ?? undefined,
            })
        }
    }, [isOpen, preselectedMilestoneId, reset])

    const onSubmit = (data: CreateEntregableFormData) => {
        mutation.mutate({
            titulo: data.titulo,
            descripcion: data.descripcion || undefined,
            urlRecurso: data.urlRecurso || undefined,
            milestoneId: data.milestoneId || undefined,
        })
    }

    return (
        <Dialog open={isOpen} onOpenChange={(open) => !open && onClose()}>
            <DialogContent className="max-w-md bg-[#0f1729] border-[#334155] text-white">
                <DialogHeader>
                    <DialogTitle className="text-lg font-semibold text-white flex items-center gap-2">
                        <Upload className="h-5 w-5 text-blue-400" />
                        Subir entregable
                    </DialogTitle>
                    <DialogDescription className="text-sm text-slate-400">
                        Sube un recurso para que el artista lo revise
                    </DialogDescription>
                </DialogHeader>

                <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
                    <div className="space-y-2">
                        <Label htmlFor="titulo" className="text-slate-300">
                            Titulo
                        </Label>
                        <Input
                            id="titulo"
                            placeholder="Nombre del entregable"
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
                            placeholder="Describe brevemente el entregable"
                            className="bg-slate-800 border-slate-600 text-white resize-none"
                            rows={2}
                            {...register("descripcion")}
                        />
                    </div>

                    <div className="space-y-2">
                        <Label htmlFor="urlRecurso" className="text-slate-300">
                            URL del recurso (opcional)
                        </Label>
                        <Input
                            id="urlRecurso"
                            type="url"
                            placeholder="https://drive.google.com/..."
                            className="bg-slate-800 border-slate-600 text-white"
                            {...register("urlRecurso")}
                        />
                        {errors.urlRecurso && (
                            <p className="text-xs text-red-400">
                                {errors.urlRecurso.message}
                            </p>
                        )}
                        <div className="flex items-start gap-1.5 text-xs text-slate-500">
                            <Info className="h-3 w-3 mt-0.5 shrink-0" />
                            <span>
                                Acepta enlaces de Google Drive, Dropbox, WeTransfer u otros
                                servicios de almacenamiento
                            </span>
                        </div>
                    </div>

                    <div className="space-y-2">
                        <Label className="text-slate-300">
                            Milestone (opcional)
                        </Label>
                        <Select
                            defaultValue={preselectedMilestoneId ?? "none"}
                            onValueChange={(value) =>
                                setValue(
                                    "milestoneId",
                                    value === "none" ? undefined : value
                                )
                            }
                        >
                            <SelectTrigger className="bg-slate-800 border-slate-600 text-white">
                                <SelectValue placeholder="Sin milestone" />
                            </SelectTrigger>
                            <SelectContent className="bg-slate-800 border-slate-600">
                                <SelectItem value="none" className="text-slate-300">
                                    Sin milestone
                                </SelectItem>
                                {milestones.map((m) => (
                                    <SelectItem
                                        key={m.id}
                                        value={m.id}
                                        className="text-slate-300"
                                    >
                                        {m.titulo}
                                    </SelectItem>
                                ))}
                            </SelectContent>
                        </Select>
                    </div>

                    <DialogFooter className="pt-2">
                        <Button
                            type="button"
                            variant="outline"
                            disabled={mutation.isPending}
                            className="border-slate-600 text-white hover:bg-slate-800"
                            onClick={onClose}
                        >
                            Cancelar
                        </Button>
                        <Button
                            type="submit"
                            disabled={mutation.isPending}
                            className="bg-blue-600 hover:bg-blue-700"
                        >
                            {mutation.isPending ? (
                                <>
                                    <Loader2 className="w-4 h-4 mr-2 animate-spin" />
                                    Subiendo...
                                </>
                            ) : (
                                "Subir entregable"
                            )}
                        </Button>
                    </DialogFooter>
                </form>
            </DialogContent>
        </Dialog>
    )
}
