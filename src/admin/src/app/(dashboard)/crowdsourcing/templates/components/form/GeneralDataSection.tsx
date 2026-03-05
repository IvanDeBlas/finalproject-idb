"use client"

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Textarea } from "@/components/ui/textarea"
import { Label } from "@/components/ui/label"
import { Checkbox } from "@/components/ui/checkbox"
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from "@/components/ui/select"
import { useFormContext, Controller } from "react-hook-form"
import { IconPreview, AVAILABLE_ICONS } from "../shared/IconPreview"
import type { CreateTemplateFormData } from "@/lib/validations/template.schema"

export function GeneralDataSection() {
    const {
        register,
        control,
        watch,
        formState: { errors },
    } = useFormContext<CreateTemplateFormData>()

    const selectedIcon = watch("icono")

    return (
        <Card>
            <CardHeader>
                <CardTitle>Datos Generales</CardTitle>
            </CardHeader>
            <CardContent className="space-y-4">
                <div>
                    <Label htmlFor="nombre">Nombre *</Label>
                    <Input
                        id="nombre"
                        {...register("nombre")}
                        placeholder="Ej: Produccion de EP"
                    />
                    {errors.nombre && (
                        <p className="text-sm text-destructive mt-1">
                            {errors.nombre.message}
                        </p>
                    )}
                </div>

                <div>
                    <Label htmlFor="descripcion">Descripcion</Label>
                    <Textarea
                        id="descripcion"
                        {...register("descripcion")}
                        placeholder="Descripcion del template..."
                        className="min-h-[100px]"
                    />
                    {errors.descripcion && (
                        <p className="text-sm text-destructive mt-1">
                            {errors.descripcion.message}
                        </p>
                    )}
                </div>

                <div>
                    <Label>Icono *</Label>
                    <div className="flex gap-4 items-center">
                        <Controller
                            name="icono"
                            control={control}
                            render={({ field }) => (
                                <Select
                                    value={field.value}
                                    onValueChange={field.onChange}
                                >
                                    <SelectTrigger className="w-48">
                                        <SelectValue placeholder="Selecciona icono" />
                                    </SelectTrigger>
                                    <SelectContent>
                                        {AVAILABLE_ICONS.map((icon) => (
                                            <SelectItem key={icon} value={icon}>
                                                {icon}
                                            </SelectItem>
                                        ))}
                                    </SelectContent>
                                </Select>
                            )}
                        />
                        {selectedIcon && <IconPreview icon={selectedIcon} size="lg" />}
                    </div>
                    {errors.icono && (
                        <p className="text-sm text-destructive mt-1">
                            {errors.icono.message}
                        </p>
                    )}
                </div>

                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <div>
                        <Label htmlFor="orden">Orden</Label>
                        <Input
                            id="orden"
                            type="number"
                            {...register("orden", { valueAsNumber: true })}
                            className="w-24"
                        />
                        {errors.orden && (
                            <p className="text-sm text-destructive mt-1">
                                {errors.orden.message}
                            </p>
                        )}
                    </div>
                    <div className="flex items-center gap-2 pt-6">
                        <Controller
                            name="activo"
                            control={control}
                            render={({ field }) => (
                                <Checkbox
                                    id="activo"
                                    checked={field.value}
                                    onCheckedChange={field.onChange}
                                />
                            )}
                        />
                        <Label htmlFor="activo" className="cursor-pointer">
                            Activo
                        </Label>
                    </div>
                </div>
            </CardContent>
        </Card>
    )
}
