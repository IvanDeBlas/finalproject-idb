"use client"

import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Separator } from "@/components/ui/separator"
import { useUpdatePromotor } from "@/hooks/use-promotor-mutations"
import { updatePromotorSchema } from "@shared/schemas"
import { TIPO_PROMOTOR_LABELS } from "@shared/constants"
import { mapPromotorToUpdateForm } from "@shared/utils/mappers"
import {
    Lock,
    Instagram,
    Youtube,
    AlertTriangle,
} from "lucide-react"
import type { UpdatePromotorFormData } from "@shared/schemas"
import type { Promotor } from "@shared/types"

interface PromotorEditFormProps {
    promotor: Promotor
    onSuccess: () => void
    onCancel: () => void
}

export function PromotorEditForm({ promotor, onSuccess, onCancel }: PromotorEditFormProps) {
    const updateMutation = useUpdatePromotor()

    const {
        register,
        handleSubmit,
        formState: { errors, isDirty },
    } = useForm<UpdatePromotorFormData>({
        resolver: zodResolver(updatePromotorSchema),
        defaultValues: mapPromotorToUpdateForm(promotor),
    })

    const tipoLabel = TIPO_PROMOTOR_LABELS[promotor.tipoPromotorId] ?? promotor.tipoPromotorNombre

    const onSubmit = (data: UpdatePromotorFormData) => {
        updateMutation.mutate(data, {
            onSuccess: () => {
                onSuccess()
            },
        })
    }

    return (
        <form onSubmit={handleSubmit(onSubmit)} className="max-w-2xl mx-auto space-y-6">
            {/* Public Information */}
            <Card>
                <CardHeader>
                    <CardTitle>Informacion publica</CardTitle>
                </CardHeader>
                <CardContent className="space-y-4">
                    <div className="space-y-2">
                        <Label htmlFor="nombrePublico">
                            Nombre publico <span className="text-red-400">*</span>
                        </Label>
                        <Input
                            id="nombrePublico"
                            {...register("nombrePublico")}
                            aria-required="true"
                            aria-invalid={!!errors.nombrePublico}
                            aria-describedby={errors.nombrePublico ? "nombrePublico-error" : undefined}
                        />
                        {errors.nombrePublico && (
                            <p
                                id="nombrePublico-error"
                                className="text-sm text-red-400"
                                role="alert"
                                aria-live="polite"
                            >
                                {errors.nombrePublico.message}
                            </p>
                        )}
                    </div>

                    <div className="space-y-2">
                        <Label htmlFor="tipoPromotor">Tipo de promotor</Label>
                        <div className="relative">
                            <Input
                                id="tipoPromotor"
                                value={tipoLabel}
                                disabled
                                aria-disabled="true"
                                className="pr-10"
                            />
                            <Lock className="absolute right-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" aria-hidden="true" />
                        </div>
                        <p className="text-xs text-muted-foreground">
                            El tipo de promotor no se puede modificar despues del registro
                        </p>
                    </div>

                    <div className="space-y-2">
                        <Label htmlFor="emailContacto">Email de contacto</Label>
                        <Input
                            id="emailContacto"
                            type="email"
                            {...register("emailContacto")}
                            aria-invalid={!!errors.emailContacto}
                            aria-describedby={errors.emailContacto ? "emailContacto-error" : undefined}
                        />
                        {errors.emailContacto && (
                            <p
                                id="emailContacto-error"
                                className="text-sm text-red-400"
                                role="alert"
                                aria-live="polite"
                            >
                                {errors.emailContacto.message}
                            </p>
                        )}
                    </div>

                    <div className="space-y-2">
                        <Label htmlFor="urlSitioWeb">Sitio web</Label>
                        <Input
                            id="urlSitioWeb"
                            type="url"
                            placeholder="https://"
                            {...register("urlSitioWeb")}
                            aria-invalid={!!errors.urlSitioWeb}
                            aria-describedby={errors.urlSitioWeb ? "urlSitioWeb-error" : undefined}
                        />
                        {errors.urlSitioWeb && (
                            <p
                                id="urlSitioWeb-error"
                                className="text-sm text-red-400"
                                role="alert"
                                aria-live="polite"
                            >
                                {errors.urlSitioWeb.message}
                            </p>
                        )}
                    </div>
                </CardContent>
            </Card>

            {/* Social Links */}
            <Card>
                <CardHeader>
                    <CardTitle>Redes sociales</CardTitle>
                </CardHeader>
                <CardContent className="space-y-4">
                    <div className="space-y-2">
                        <Label htmlFor="urlInstagram" className="flex items-center gap-2">
                            <Instagram className="h-4 w-4 text-pink-400" aria-hidden="true" />
                            Instagram
                        </Label>
                        <Input
                            id="urlInstagram"
                            placeholder="https://instagram.com/..."
                            {...register("urlInstagram")}
                            aria-invalid={!!errors.urlInstagram}
                            aria-describedby={errors.urlInstagram ? "urlInstagram-error" : undefined}
                        />
                        {errors.urlInstagram && (
                            <p id="urlInstagram-error" className="text-sm text-red-400" role="alert" aria-live="polite">
                                {errors.urlInstagram.message}
                            </p>
                        )}
                    </div>

                    <div className="space-y-2">
                        <Label htmlFor="urlTikTok" className="flex items-center gap-2">
                            <svg className="h-4 w-4 text-slate-400" viewBox="0 0 24 24" fill="currentColor" aria-hidden="true">
                                <path d="M19.59 6.69a4.83 4.83 0 01-3.77-4.25V2h-3.45v13.67a2.89 2.89 0 01-2.88 2.5 2.89 2.89 0 01-2.88-2.88 2.89 2.89 0 012.88-2.88c.28 0 .54.04.79.1v-3.5a6.37 6.37 0 00-.79-.05A6.34 6.34 0 003.15 15.2a6.34 6.34 0 006.34 6.34 6.34 6.34 0 006.34-6.34V8.98a8.21 8.21 0 004.76 1.52V7.05a4.84 4.84 0 01-1-.36z" />
                            </svg>
                            TikTok
                        </Label>
                        <Input
                            id="urlTikTok"
                            placeholder="https://tiktok.com/@..."
                            {...register("urlTikTok")}
                            aria-invalid={!!errors.urlTikTok}
                            aria-describedby={errors.urlTikTok ? "urlTikTok-error" : undefined}
                        />
                        {errors.urlTikTok && (
                            <p id="urlTikTok-error" className="text-sm text-red-400" role="alert" aria-live="polite">
                                {errors.urlTikTok.message}
                            </p>
                        )}
                    </div>

                    <div className="space-y-2">
                        <Label htmlFor="urlYouTube" className="flex items-center gap-2">
                            <Youtube className="h-4 w-4 text-red-400" aria-hidden="true" />
                            YouTube
                        </Label>
                        <Input
                            id="urlYouTube"
                            placeholder="https://youtube.com/..."
                            {...register("urlYouTube")}
                            aria-invalid={!!errors.urlYouTube}
                            aria-describedby={errors.urlYouTube ? "urlYouTube-error" : undefined}
                        />
                        {errors.urlYouTube && (
                            <p id="urlYouTube-error" className="text-sm text-red-400" role="alert" aria-live="polite">
                                {errors.urlYouTube.message}
                            </p>
                        )}
                    </div>

                    <div className="space-y-2">
                        <Label htmlFor="urlTwitter" className="flex items-center gap-2">
                            <svg className="h-4 w-4 text-sky-400" viewBox="0 0 24 24" fill="currentColor" aria-hidden="true">
                                <path d="M18.244 2.25h3.308l-7.227 8.26 8.502 11.24H16.17l-5.214-6.817L4.99 21.75H1.68l7.73-8.835L1.254 2.25H8.08l4.713 6.231zm-1.161 17.52h1.833L7.084 4.126H5.117z" />
                            </svg>
                            Twitter / X
                        </Label>
                        <Input
                            id="urlTwitter"
                            placeholder="https://x.com/..."
                            {...register("urlTwitter")}
                            aria-invalid={!!errors.urlTwitter}
                            aria-describedby={errors.urlTwitter ? "urlTwitter-error" : undefined}
                        />
                        {errors.urlTwitter && (
                            <p id="urlTwitter-error" className="text-sm text-red-400" role="alert" aria-live="polite">
                                {errors.urlTwitter.message}
                            </p>
                        )}
                    </div>
                </CardContent>
            </Card>

            {/* Danger Zone - only if active */}
            {promotor.esActivo && (
                <Card className="border-red-900/50">
                    <CardContent className="pt-6">
                        <div className="flex items-center gap-2 mb-2">
                            <AlertTriangle className="h-4 w-4 text-red-400" aria-hidden="true" />
                            <h3 className="text-sm font-medium text-red-400">Zona de peligro</h3>
                        </div>
                        <p className="text-sm text-muted-foreground mb-4">
                            Al desactivar tu perfil, se daran de baja todos tus programas activos.
                        </p>
                    </CardContent>
                </Card>
            )}

            {/* Form Actions */}
            <Separator />
            <div className="flex items-center justify-end gap-3">
                <Button type="button" variant="ghost" onClick={onCancel}>
                    Cancelar
                </Button>
                <Button
                    type="submit"
                    disabled={!isDirty || updateMutation.isPending}
                    className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700"
                >
                    {updateMutation.isPending ? "Guardando..." : "Guardar cambios"}
                </Button>
            </div>
        </form>
    )
}
