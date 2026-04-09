import { FC, useMemo } from "react"
import { useForm, Controller } from "react-hook-form"
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
import { CheckCircle, AlertTriangle, Info, Loader2 } from "lucide-react"
import { createPropuestaSchema, type CreatePropuestaFormData } from "@shared/schemas/crowdsourcing.schema"
import { useCreatePropuesta } from "../../application"

interface EnviarPropuestaFormProps {
    necesidadId: string
    necesidadTitulo: string
    presupuestoMin?: number
    presupuestoMax?: number
    monedaNombre?: string
    isOpen: boolean
    onClose: () => void
}

const MONEDAS = [
    { id: 1, nombre: "EUR" },
    { id: 2, nombre: "USD" },
]

type PriceInfo = "dentro" | "por-encima" | "por-debajo" | null

export const EnviarPropuestaForm: FC<EnviarPropuestaFormProps> = ({
    necesidadId,
    necesidadTitulo,
    presupuestoMin,
    presupuestoMax,
    monedaNombre,
    isOpen,
    onClose,
}) => {
    const { mutate, isPending } = useCreatePropuesta(necesidadId, onClose)

    const form = useForm<CreatePropuestaFormData>({
        resolver: zodResolver(createPropuestaSchema),
        defaultValues: {
            precioPropuesto: undefined,
            monedaId: 1,
            diasEstimados: undefined,
            mensajePropuesta: "",
        },
    })

    const {
        register,
        control,
        handleSubmit,
        watch,
        reset,
        formState: { errors },
    } = form

    const precioPropuesto = watch("precioPropuesto")
    const mensajePropuesta = watch("mensajePropuesta") ?? ""

    const precioInfo: PriceInfo = useMemo(() => {
        if (!precioPropuesto || precioPropuesto <= 0) return null
        if (presupuestoMin === undefined || presupuestoMax === undefined) return null
        if (precioPropuesto >= presupuestoMin && precioPropuesto <= presupuestoMax)
            return "dentro"
        if (precioPropuesto > presupuestoMax) return "por-encima"
        if (precioPropuesto < presupuestoMin) return "por-debajo"
        return null
    }, [precioPropuesto, presupuestoMin, presupuestoMax])

    const onSubmit = (data: CreatePropuestaFormData) => {
        mutate(data)
    }

    const handleClose = () => {
        reset()
        onClose()
    }

    return (
        <Dialog open={isOpen} onOpenChange={(open) => !open && handleClose()}>
            <DialogContent className="max-w-lg bg-[#0f1729] border-[#334155] text-white">
                <DialogHeader className="pb-4 border-b border-[#334155]">
                    <DialogTitle className="text-xl font-semibold text-white">
                        Enviar propuesta
                    </DialogTitle>
                    <DialogDescription className="text-sm text-[#94a3b8] mt-1">
                        Para: {necesidadTitulo}
                    </DialogDescription>
                    {presupuestoMin !== undefined && presupuestoMax !== undefined && (
                        <p className="text-xs text-[#64748b] mt-1">
                            Presupuesto del artista: {presupuestoMin} - {presupuestoMax}{" "}
                            {monedaNombre ?? "EUR"}
                        </p>
                    )}
                </DialogHeader>

                <form onSubmit={handleSubmit(onSubmit)} className="py-6 space-y-5">
                    <div className="grid grid-cols-2 gap-3">
                        <div>
                            <Label
                                htmlFor="precioPropuesto"
                                className="text-sm font-medium text-[#cbd5e1] mb-1.5 block"
                            >
                                Precio propuesto
                                <span aria-hidden="true" className="text-red-400 ml-1">*</span>
                            </Label>
                            <Input
                                id="precioPropuesto"
                                type="number"
                                step="0.01"
                                aria-required="true"
                                aria-invalid={!!errors.precioPropuesto}
                                aria-describedby="precio-error"
                                disabled={isPending}
                                className="bg-[#1a1a2e] border-[#334155] text-white h-11 focus:border-[#a855f7]"
                                {...register("precioPropuesto", { valueAsNumber: true })}
                            />
                            {errors.precioPropuesto && (
                                <p id="precio-error" role="alert" className="text-sm text-red-400 mt-1">
                                    {errors.precioPropuesto.message}
                                </p>
                            )}
                        </div>
                        <div>
                            <Label
                                htmlFor="monedaId"
                                className="text-sm font-medium text-[#cbd5e1] mb-1.5 block"
                            >
                                Moneda
                                <span aria-hidden="true" className="text-red-400 ml-1">*</span>
                            </Label>
                            <Controller
                                name="monedaId"
                                control={control}
                                render={({ field }) => (
                                    <Select
                                        value={field.value?.toString()}
                                        onValueChange={(val) => field.onChange(Number(val))}
                                        disabled={isPending}
                                    >
                                        <SelectTrigger className="bg-[#1a1a2e] border-[#334155] text-white h-11">
                                            <SelectValue placeholder="Seleccionar" />
                                        </SelectTrigger>
                                        <SelectContent className="bg-[#0f1729] border-[#334155]">
                                            {MONEDAS.map((m) => (
                                                <SelectItem key={m.id} value={m.id.toString()}>
                                                    {m.nombre}
                                                </SelectItem>
                                            ))}
                                        </SelectContent>
                                    </Select>
                                )}
                            />
                        </div>
                    </div>

                    {precioInfo === "dentro" && (
                        <div className="bg-green-900/20 border border-green-700/50 rounded-md p-3 flex items-center gap-2">
                            <CheckCircle className="w-4 h-4 text-green-400 flex-shrink-0" aria-hidden="true" />
                            <p className="text-sm text-green-300">
                                Tu precio esta dentro del rango ({presupuestoMin} - {presupuestoMax}{" "}
                                {monedaNombre ?? "EUR"})
                            </p>
                        </div>
                    )}
                    {precioInfo === "por-encima" && (
                        <div className="bg-amber-900/20 border border-amber-700/50 rounded-md p-3 flex items-center gap-2">
                            <AlertTriangle className="w-4 h-4 text-amber-400 flex-shrink-0" aria-hidden="true" />
                            <p className="text-sm text-amber-300">
                                Tu precio esta por encima del presupuesto indicado (max{" "}
                                {presupuestoMax} {monedaNombre ?? "EUR"})
                            </p>
                        </div>
                    )}
                    {precioInfo === "por-debajo" && (
                        <div className="bg-blue-900/20 border border-blue-700/50 rounded-md p-3 flex items-center gap-2">
                            <Info className="w-4 h-4 text-blue-400 flex-shrink-0" aria-hidden="true" />
                            <p className="text-sm text-blue-300">
                                Tu precio esta por debajo del presupuesto indicado (min{" "}
                                {presupuestoMin} {monedaNombre ?? "EUR"})
                            </p>
                        </div>
                    )}

                    <div>
                        <Label
                            htmlFor="diasEstimados"
                            className="text-sm font-medium text-[#cbd5e1] mb-1.5 block"
                        >
                            Dias estimados
                        </Label>
                        <Input
                            id="diasEstimados"
                            type="number"
                            disabled={isPending}
                            className="bg-[#1a1a2e] border-[#334155] text-white h-11 w-40 focus:border-[#a855f7]"
                            {...register("diasEstimados", { valueAsNumber: true })}
                        />
                        <p className="text-xs text-[#64748b] mt-1">
                            Opcional - cuantos dias necesitas para completar el trabajo
                        </p>
                        {errors.diasEstimados && (
                            <p role="alert" className="text-sm text-red-400 mt-1">
                                {errors.diasEstimados.message}
                            </p>
                        )}
                    </div>

                    <div>
                        <Label
                            htmlFor="mensajePropuesta"
                            className="text-sm font-medium text-[#cbd5e1] mb-1.5 block"
                        >
                            Mensaje de propuesta
                            <span aria-hidden="true" className="text-red-400 ml-1">*</span>
                        </Label>
                        <Textarea
                            id="mensajePropuesta"
                            aria-required="true"
                            aria-invalid={!!errors.mensajePropuesta}
                            aria-describedby="mensaje-counter"
                            disabled={isPending}
                            className="bg-[#1a1a2e] border-[#334155] text-white min-h-[140px] resize-none focus:border-[#a855f7]"
                            {...register("mensajePropuesta")}
                        />
                        <div id="mensaje-counter" className="flex justify-between mt-1">
                            <span
                                aria-live="polite"
                                aria-atomic="true"
                                className={`text-xs ${
                                    mensajePropuesta.length > 1900
                                        ? "text-red-400"
                                        : "text-[#64748b]"
                                }`}
                            >
                                {mensajePropuesta.length} / 2000 (min 20)
                            </span>
                        </div>
                        {errors.mensajePropuesta && (
                            <p role="alert" className="text-sm text-red-400 mt-1">
                                {errors.mensajePropuesta.message}
                            </p>
                        )}
                    </div>

                    <DialogFooter className="pt-4 border-t border-[#334155]">
                        <Button
                            type="button"
                            variant="outline"
                            disabled={isPending}
                            className="border-[#334155] text-white hover:bg-[#1e2a42]"
                            onClick={handleClose}
                        >
                            Cancelar
                        </Button>
                        <Button
                            type="submit"
                            disabled={isPending}
                            className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 disabled:opacity-50"
                        >
                            {isPending ? (
                                <>
                                    <Loader2 className="w-4 h-4 mr-2 animate-spin" aria-hidden="true" />
                                    Enviando...
                                </>
                            ) : (
                                "Enviar propuesta"
                            )}
                        </Button>
                    </DialogFooter>
                </form>
            </DialogContent>
        </Dialog>
    )
}
