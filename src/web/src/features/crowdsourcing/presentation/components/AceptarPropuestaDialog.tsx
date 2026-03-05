import { FC, useEffect } from "react"
import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import { useNavigate } from "react-router-dom"
import {
    Dialog,
    DialogContent,
    DialogHeader,
    DialogTitle,
    DialogDescription,
    DialogFooter,
} from "@/components/ui/dialog"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Button } from "@/components/ui/button"
import { Info, Loader2 } from "lucide-react"
import { APP_ROUTES } from "@shared/constants"
import { aceptarPropuestaSchema } from "@shared/schemas/crowdsourcing.schema"
import type { AceptarPropuestaFormData } from "@shared/schemas/crowdsourcing.schema"
import { useAceptarPropuesta } from "../../application/hooks/useAceptarPropuesta"
import type { PropuestaCrowdsourcing } from "@shared/types/crowdsourcing"

interface AceptarPropuestaDialogProps {
    propuesta: PropuestaCrowdsourcing | null
    necesidadTitulo: string
    diasEstimados?: number
    isOpen: boolean
    onClose: () => void
}

function todayStr(): string {
    return new Date().toISOString().split("T")[0]
}

function addDays(dateStr: string, days: number): string {
    const d = new Date(dateStr)
    d.setDate(d.getDate() + days)
    return d.toISOString().split("T")[0]
}

export const AceptarPropuestaDialog: FC<AceptarPropuestaDialogProps> = ({
    propuesta,
    necesidadTitulo,
    diasEstimados,
    isOpen,
    onClose,
}) => {
    const navigate = useNavigate()
    const mutation = useAceptarPropuesta(
        propuesta?.id ?? "",
        (acuerdoId) => {
            onClose()
            navigate(APP_ROUTES.landing.crowdsourcing.acuerdoDetail(acuerdoId))
        }
    )

    const {
        register,
        handleSubmit,
        reset,
        formState: { errors },
    } = useForm<AceptarPropuestaFormData>({
        resolver: zodResolver(aceptarPropuestaSchema),
    })

    useEffect(() => {
        if (isOpen) {
            const today = todayStr()
            reset({
                tituloInterno: necesidadTitulo,
                fechaInicio: today,
                fechaFinPrevista: diasEstimados
                    ? addDays(today, diasEstimados)
                    : "",
            })
        }
    }, [isOpen, necesidadTitulo, diasEstimados, reset])

    const onSubmit = (data: AceptarPropuestaFormData) => {
        mutation.mutate({
            tituloInterno: data.tituloInterno,
            fechaInicio: data.fechaInicio,
            fechaFinPrevista: data.fechaFinPrevista || undefined,
        })
    }

    return (
        <Dialog open={isOpen} onOpenChange={(open) => !open && onClose()}>
            <DialogContent className="max-w-md bg-[#0f1729] border-[#334155] text-white">
                <DialogHeader>
                    <DialogTitle className="text-lg font-semibold text-white">
                        Aceptar propuesta
                    </DialogTitle>
                    <DialogDescription className="text-sm text-slate-400">
                        Propuesta de{" "}
                        <span className="text-white font-medium">
                            {propuesta?.profesionalNombre}
                        </span>{" "}
                        por {propuesta?.precioPropuesto.toLocaleString("es-ES", { minimumFractionDigits: 2 })} EUR
                    </DialogDescription>
                </DialogHeader>

                <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
                    <div className="space-y-2">
                        <Label htmlFor="tituloInterno" className="text-slate-300">
                            Titulo interno del acuerdo
                        </Label>
                        <Input
                            id="tituloInterno"
                            className="bg-slate-800 border-slate-600 text-white"
                            {...register("tituloInterno")}
                        />
                        {errors.tituloInterno && (
                            <p className="text-xs text-red-400">
                                {errors.tituloInterno.message}
                            </p>
                        )}
                    </div>

                    <div className="grid grid-cols-2 gap-3">
                        <div className="space-y-2">
                            <Label htmlFor="fechaInicio" className="text-slate-300">
                                Fecha de inicio
                            </Label>
                            <Input
                                id="fechaInicio"
                                type="date"
                                className="bg-slate-800 border-slate-600 text-white"
                                {...register("fechaInicio")}
                            />
                            {errors.fechaInicio && (
                                <p className="text-xs text-red-400">
                                    {errors.fechaInicio.message}
                                </p>
                            )}
                        </div>
                        <div className="space-y-2">
                            <Label htmlFor="fechaFinPrevista" className="text-slate-300">
                                Fin previsto (opcional)
                            </Label>
                            <Input
                                id="fechaFinPrevista"
                                type="date"
                                className="bg-slate-800 border-slate-600 text-white"
                                {...register("fechaFinPrevista")}
                            />
                            {errors.fechaFinPrevista && (
                                <p className="text-xs text-red-400">
                                    {errors.fechaFinPrevista.message}
                                </p>
                            )}
                        </div>
                    </div>

                    <div className="flex items-start gap-1.5 text-xs text-slate-500 bg-slate-800/50 rounded p-2">
                        <Info className="h-3 w-3 mt-0.5 shrink-0" />
                        <span>
                            Al aceptar, las demas propuestas pendientes seran rechazadas
                            automaticamente
                        </span>
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
                            className="bg-green-600 hover:bg-green-700"
                        >
                            {mutation.isPending ? (
                                <>
                                    <Loader2 className="w-4 h-4 mr-2 animate-spin" />
                                    Creando acuerdo...
                                </>
                            ) : (
                                "Aceptar y crear acuerdo"
                            )}
                        </Button>
                    </DialogFooter>
                </form>
            </DialogContent>
        </Dialog>
    )
}
