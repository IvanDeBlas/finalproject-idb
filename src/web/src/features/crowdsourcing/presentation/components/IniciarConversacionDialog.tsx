import { FC } from "react"
import { useNavigate } from "react-router-dom"
import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import { toast } from "sonner"
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
import { Briefcase, FileText, Loader2 } from "lucide-react"
import {
    createConversacionSchema,
    type CreateConversacionFormData,
} from "@shared/schemas/crowdsourcing.schema"
import { getMensajeriaErrorMessage } from "@shared/utils/error-messages"
import { useCreateConversacion } from "../../application/hooks/useCreateConversacion"
import type { ContextoConversacion } from "../../domain"

interface IniciarConversacionDialogProps {
    isOpen: boolean
    onClose: () => void
    destinatarioId: string
    destinatarioNombre: string
    contextoId: string
    contextoTipo: ContextoConversacion
    contextoTitulo: string
}

export const IniciarConversacionDialog: FC<IniciarConversacionDialogProps> = ({
    isOpen,
    onClose,
    destinatarioId,
    destinatarioNombre,
    contextoId,
    contextoTipo,
    contextoTitulo,
}) => {
    const navigate = useNavigate()
    const createConversacion = useCreateConversacion()

    const {
        register,
        handleSubmit,
        reset,
        watch,
        formState: { errors },
    } = useForm<CreateConversacionFormData>({
        resolver: zodResolver(createConversacionSchema),
        defaultValues: {
            asunto: "",
            userIdDestinatario: destinatarioId,
            necesidadId: contextoTipo === "necesidad" ? contextoId : undefined,
            acuerdoId: contextoTipo === "acuerdo" ? contextoId : undefined,
        },
    })

    const asuntoValue = watch("asunto") ?? ""

    const onSubmit = (data: CreateConversacionFormData) => {
        createConversacion.mutate(data, {
            onSuccess: (result) => {
                toast.success("Conversacion iniciada correctamente.")
                reset()
                onClose()
                navigate(`/crowdsourcing/mensajes/${result.id}`)
            },
            onError: (error: Error) => {
                if (error.message === "4015") {
                    toast.info(getMensajeriaErrorMessage("4015"))
                    reset()
                    onClose()
                    navigate("/crowdsourcing/mensajes")
                } else {
                    toast.error(getMensajeriaErrorMessage(error.message))
                }
            },
        })
    }

    const handleOpenChange = (open: boolean) => {
        if (!open) {
            reset()
            onClose()
        }
    }

    return (
        <Dialog open={isOpen} onOpenChange={handleOpenChange}>
            <DialogContent className="max-w-md bg-[#0f1729] border-[#334155] text-white">
                <DialogHeader>
                    <DialogTitle className="text-xl font-semibold text-white">
                        Iniciar conversacion
                    </DialogTitle>
                    <DialogDescription className="text-sm text-[#94a3b8] mt-0.5">
                        Con: {destinatarioNombre}
                    </DialogDescription>
                </DialogHeader>

                <div className="bg-[#16213e] border border-[#334155] rounded-lg p-3 mb-5 flex items-start gap-3">
                    {contextoTipo === "necesidad" ? (
                        <Briefcase className="w-5 h-5 text-[#a855f7] flex-shrink-0 mt-0.5" />
                    ) : (
                        <FileText className="w-5 h-5 text-[#3b82f6] flex-shrink-0 mt-0.5" />
                    )}
                    <div>
                        <p className="text-sm font-medium text-white">
                            {contextoTitulo}
                        </p>
                        <p className="text-xs text-[#64748b] mt-0.5">
                            {contextoTipo === "necesidad"
                                ? "Necesidad de crowdsourcing"
                                : "Acuerdo de crowdsourcing"}
                        </p>
                    </div>
                </div>

                <form onSubmit={handleSubmit(onSubmit)}>
                    <div>
                        <Label
                            htmlFor="asunto-input"
                            className="text-sm font-medium text-[#cbd5e1] mb-1.5 block"
                        >
                            Asunto
                            <span
                                aria-hidden="true"
                                className="text-red-400 ml-1"
                            >
                                *
                            </span>
                        </Label>
                        <Input
                            {...register("asunto")}
                            id="asunto-input"
                            autoFocus
                            placeholder="Breve descripcion del motivo..."
                            disabled={createConversacion.isPending}
                            className={`bg-[#1a1a2e] border-[#334155] text-white h-11 focus:border-[#a855f7] focus-visible:ring-[#a855f7] placeholder:text-[#64748b] ${
                                errors.asunto
                                    ? "border-red-500 focus:border-red-500"
                                    : ""
                            }`}
                        />
                        <div className="flex justify-between mt-1">
                            <p className="text-xs text-[#64748b]">
                                Breve descripcion del motivo de la conversacion
                            </p>
                            <span className="text-xs text-[#64748b]">
                                {asuntoValue.length} / 200
                            </span>
                        </div>
                        {errors.asunto && (
                            <p
                                role="alert"
                                className="text-sm text-red-400 mt-1"
                            >
                                {errors.asunto.message}
                            </p>
                        )}
                    </div>

                    <DialogFooter className="pt-4 border-t border-[#334155] flex justify-between gap-3 mt-4">
                        <Button
                            type="button"
                            variant="outline"
                            onClick={() => handleOpenChange(false)}
                            disabled={createConversacion.isPending}
                            className="border-[#334155] text-white hover:bg-[#1e2a42]"
                        >
                            Cancelar
                        </Button>
                        <Button
                            type="submit"
                            disabled={
                                createConversacion.isPending ||
                                !!errors.asunto
                            }
                            className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700"
                        >
                            {createConversacion.isPending ? (
                                <>
                                    <Loader2 className="animate-spin w-4 h-4 mr-2" />
                                    Iniciando...
                                </>
                            ) : (
                                "Iniciar conversacion"
                            )}
                        </Button>
                    </DialogFooter>
                </form>
            </DialogContent>
        </Dialog>
    )
}
