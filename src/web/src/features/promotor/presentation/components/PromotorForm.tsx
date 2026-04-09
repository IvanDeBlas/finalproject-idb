import { useState, type FC } from "react"
import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Button } from "@/components/ui/button"
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from "@/components/ui/select"
import { Loader2, Lock, Info, AlertCircle } from "lucide-react"
import {
    createPromotorSchema,
    updatePromotorSchema,
    type CreatePromotorFormData,
} from "@shared/schemas/crowdpromotion.schema"
import { TIPO_PROMOTOR_LABELS, TIPO_PROMOTOR_DESCRIPTIONS } from "@shared/constants"
import { PromotorSocialFields } from "./PromotorSocialFields"

interface PromotorFormProps {
    mode: "create" | "edit"
    defaultValues?: Partial<CreatePromotorFormData>
    onSubmit: (data: CreatePromotorFormData) => Promise<void>
    isSubmitting: boolean
    tipoPromotorNombre?: string
    onCancel: () => void
}

export const PromotorForm: FC<PromotorFormProps> = ({
    mode,
    defaultValues,
    onSubmit,
    isSubmitting,
    tipoPromotorNombre,
    onCancel,
}) => {
    const [showSocialWarning, setShowSocialWarning] = useState(false)

    const schema = mode === "create" ? createPromotorSchema : updatePromotorSchema

    const {
        register,
        handleSubmit,
        setValue,
        watch,
        formState: { errors },
    } = useForm<CreatePromotorFormData>({
        resolver: zodResolver(schema),
        defaultValues: defaultValues ?? {},
    })

    const handleSocialBlur = () => {
        if (mode !== "create") return
        const values = watch()
        const hasAnySocial =
            values.urlInstagram ||
            values.urlTikTok ||
            values.urlYouTube ||
            values.urlTwitter
        setShowSocialWarning(!hasAnySocial)
    }

    return (
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">
            {/* Nombre publico */}
            <div className="space-y-1.5">
                <Label htmlFor="nombrePublico" className="text-sm font-medium text-[#cbd5e1] block">
                    Nombre publico <span className="text-red-500 ml-1">*</span>
                </Label>
                <Input
                    id="nombrePublico"
                    placeholder="Tu nombre de promotor"
                    disabled={isSubmitting}
                    className="bg-[#0f0f1f] border-[#334155] text-white placeholder:text-[#64748b] focus:border-[#a855f7] h-11 focus-visible:ring-0 focus-visible:ring-offset-0"
                    {...register("nombrePublico")}
                />
                {errors.nombrePublico && (
                    <p className="text-xs text-red-400 mt-1 flex items-center gap-1" role="alert">
                        <AlertCircle className="w-3 h-3 flex-shrink-0" />
                        {errors.nombrePublico.message}
                    </p>
                )}
            </div>

            {/* Tipo de promotor */}
            {mode === "create" ? (
                <div className="space-y-1.5">
                    <Label htmlFor="tipoPromotorId" className="text-sm font-medium text-[#cbd5e1] block">
                        Tipo de promotor <span className="text-red-500 ml-1">*</span>
                    </Label>
                    <Select
                        onValueChange={(value) =>
                            setValue("tipoPromotorId", Number(value), { shouldValidate: true })
                        }
                        disabled={isSubmitting}
                    >
                        <SelectTrigger className="bg-[#0f0f1f] border-[#334155] text-white h-11 focus:ring-0 focus:ring-offset-0 focus:border-[#a855f7]">
                            <SelectValue placeholder="Selecciona un tipo" />
                        </SelectTrigger>
                        <SelectContent className="bg-[#151525] border-[#334155]">
                            {Object.entries(TIPO_PROMOTOR_LABELS).map(([id, label]) => (
                                <SelectItem
                                    key={id}
                                    value={id}
                                    className="text-white focus:bg-[#1e1e38] focus:text-white"
                                >
                                    <div>
                                        <span>{label}</span>
                                        {TIPO_PROMOTOR_DESCRIPTIONS[Number(id)] && (
                                            <span className="text-xs text-[#64748b] ml-2">
                                                - {TIPO_PROMOTOR_DESCRIPTIONS[Number(id)]}
                                            </span>
                                        )}
                                    </div>
                                </SelectItem>
                            ))}
                        </SelectContent>
                    </Select>
                    {errors.tipoPromotorId && (
                        <p className="text-xs text-red-400 mt-1 flex items-center gap-1" role="alert">
                            <AlertCircle className="w-3 h-3 flex-shrink-0" />
                            {errors.tipoPromotorId.message}
                        </p>
                    )}
                </div>
            ) : (
                <div className="space-y-1.5">
                    <Label className="text-sm font-medium text-[#cbd5e1] block">
                        Tipo de promotor
                    </Label>
                    <div className="relative">
                        <Input
                            value={tipoPromotorNombre ?? ""}
                            disabled
                            className="bg-[#1a1a2e] border-[#1e2a42] text-[#94a3b8] cursor-not-allowed h-11 pr-10"
                        />
                        <Lock className="absolute right-3 top-1/2 -translate-y-1/2 w-4 h-4 text-[#64748b]" />
                    </div>
                    <p className="text-xs text-[#64748b] mt-1">
                        El tipo de promotor no se puede cambiar despues del registro
                    </p>
                </div>
            )}

            {/* Email de contacto */}
            <div className="space-y-1.5">
                <Label htmlFor="emailContacto" className="text-sm font-medium text-[#cbd5e1] block">
                    Email de contacto
                </Label>
                <Input
                    id="emailContacto"
                    type="email"
                    placeholder="contacto@ejemplo.com"
                    disabled={isSubmitting}
                    className="bg-[#0f0f1f] border-[#334155] text-white placeholder:text-[#64748b] focus:border-[#a855f7] h-11 focus-visible:ring-0 focus-visible:ring-offset-0"
                    {...register("emailContacto")}
                />
                {errors.emailContacto && (
                    <p className="text-xs text-red-400 mt-1 flex items-center gap-1" role="alert">
                        <AlertCircle className="w-3 h-3 flex-shrink-0" />
                        {errors.emailContacto.message}
                    </p>
                )}
            </div>

            {/* Sitio web */}
            <div className="space-y-1.5">
                <Label htmlFor="urlSitioWeb" className="text-sm font-medium text-[#cbd5e1] block">
                    Sitio web
                </Label>
                <Input
                    id="urlSitioWeb"
                    type="url"
                    placeholder="https://tusitio.com"
                    disabled={isSubmitting}
                    className="bg-[#0f0f1f] border-[#334155] text-white placeholder:text-[#64748b] focus:border-[#a855f7] h-11 focus-visible:ring-0 focus-visible:ring-offset-0"
                    {...register("urlSitioWeb")}
                />
                {errors.urlSitioWeb && (
                    <p className="text-xs text-red-400 mt-1 flex items-center gap-1" role="alert">
                        <AlertCircle className="w-3 h-3 flex-shrink-0" />
                        {errors.urlSitioWeb.message}
                    </p>
                )}
            </div>

            {/* Redes sociales */}
            <PromotorSocialFields
                register={register}
                errors={errors}
                disabled={isSubmitting}
                onLastFieldBlur={handleSocialBlur}
            />

            {/* Aviso FA-03 */}
            {showSocialWarning && mode === "create" && (
                <div className="flex items-start gap-2 p-3 rounded-lg bg-blue-950/40 border border-blue-800/50 text-sm text-blue-300 animate-in fade-in duration-300">
                    <Info className="w-4 h-4 flex-shrink-0 mt-0.5" />
                    <p>
                        Agregar al menos una red social te ayudara a que los artistas
                        confien en tu perfil de promotor.
                    </p>
                </div>
            )}

            {/* Botones */}
            <div className="flex items-center justify-between pt-4">
                <Button
                    type="button"
                    variant="ghost"
                    className="text-[#94a3b8] hover:text-white hover:bg-[#1e1e38]"
                    onClick={onCancel}
                    disabled={isSubmitting}
                >
                    Cancelar
                </Button>
                <Button
                    type="submit"
                    disabled={isSubmitting}
                    className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold px-6 h-11"
                >
                    {isSubmitting ? (
                        <>
                            <Loader2 className="w-4 h-4 animate-spin mr-2" />
                            Guardando...
                        </>
                    ) : mode === "create" ? (
                        "Crear perfil"
                    ) : (
                        "Guardar cambios"
                    )}
                </Button>
            </div>
        </form>
    )
}
