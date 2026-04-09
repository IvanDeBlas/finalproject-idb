"use client"

import { Card } from "@/components/ui/card"
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
import { Trash2 } from "lucide-react"
import { useFormContext, Controller } from "react-hook-form"
import { FASES_PROYECTO } from "@shared/constants"
import type { RolProfesionalConCategoria } from "@shared/types"
import type { CreateTemplateFormData } from "@/lib/validations/template.schema"

interface NecesidadFormItemProps {
    index: number
    rolesProfesionales: RolProfesionalConCategoria[]
    onDelete: () => void
}

export function NecesidadFormItem({
    index,
    rolesProfesionales,
    onDelete,
}: NecesidadFormItemProps) {
    const {
        register,
        control,
        formState: { errors },
    } = useFormContext<CreateTemplateFormData>()

    const necErrors = errors.necesidades?.[index]

    return (
        <Card className="p-4 relative">
            <Button
                type="button"
                variant="ghost"
                size="sm"
                className="absolute top-2 right-2 text-destructive hover:text-destructive/80"
                onClick={onDelete}
                aria-label="Eliminar necesidad"
            >
                <Trash2 className="h-4 w-4" />
            </Button>

            <div className="space-y-4 pr-8">
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <div>
                        <Label htmlFor={`necesidades.${index}.fase`}>Fase *</Label>
                        <Controller
                            name={`necesidades.${index}.fase`}
                            control={control}
                            render={({ field }) => (
                                <Select
                                    value={field.value}
                                    onValueChange={field.onChange}
                                >
                                    <SelectTrigger>
                                        <SelectValue placeholder="Selecciona fase" />
                                    </SelectTrigger>
                                    <SelectContent>
                                        {FASES_PROYECTO.map((fase) => (
                                            <SelectItem key={fase} value={fase}>
                                                {fase}
                                            </SelectItem>
                                        ))}
                                    </SelectContent>
                                </Select>
                            )}
                        />
                        {necErrors?.fase && (
                            <p className="text-sm text-destructive mt-1">
                                {necErrors.fase.message}
                            </p>
                        )}
                    </div>
                    <div>
                        <Label htmlFor={`necesidades.${index}.titulo`}>Titulo *</Label>
                        <Input
                            {...register(`necesidades.${index}.titulo`)}
                            placeholder="Ej: Composicion y arreglos"
                        />
                        {necErrors?.titulo && (
                            <p className="text-sm text-destructive mt-1">
                                {necErrors.titulo.message}
                            </p>
                        )}
                    </div>
                </div>

                <div>
                    <Label htmlFor={`necesidades.${index}.rolProfesionalId`}>
                        Rol Profesional *
                    </Label>
                    <Controller
                        name={`necesidades.${index}.rolProfesionalId`}
                        control={control}
                        render={({ field }) => (
                            <Select
                                value={field.value ? String(field.value) : ""}
                                onValueChange={(v) => field.onChange(Number(v))}
                            >
                                <SelectTrigger>
                                    <SelectValue placeholder="Selecciona rol" />
                                </SelectTrigger>
                                <SelectContent>
                                    {rolesProfesionales.map((rol) => (
                                        <SelectItem key={rol.id} value={String(rol.id)}>
                                            {rol.nombre}
                                        </SelectItem>
                                    ))}
                                </SelectContent>
                            </Select>
                        )}
                    />
                    {necErrors?.rolProfesionalId && (
                        <p className="text-sm text-destructive mt-1">
                            {necErrors.rolProfesionalId.message}
                        </p>
                    )}
                </div>

                <div>
                    <Label>Presupuesto Orientativo</Label>
                    <div className="flex gap-2 items-center mt-2">
                        <Input
                            type="number"
                            {...register(`necesidades.${index}.precioMinOrientativo`, {
                                valueAsNumber: true,
                            })}
                            placeholder="Min"
                            className="w-28"
                        />
                        <span className="text-muted-foreground">-</span>
                        <Input
                            type="number"
                            {...register(`necesidades.${index}.precioMaxOrientativo`, {
                                valueAsNumber: true,
                            })}
                            placeholder="Max"
                            className="w-28"
                        />
                        <span className="text-sm text-muted-foreground">EUR</span>
                    </div>
                    {necErrors?.precioMaxOrientativo && (
                        <p className="text-sm text-destructive mt-1">
                            {necErrors.precioMaxOrientativo.message}
                        </p>
                    )}
                </div>

                <div>
                    <Label htmlFor={`necesidades.${index}.prioridad`}>Prioridad *</Label>
                    <Controller
                        name={`necesidades.${index}.prioridad`}
                        control={control}
                        render={({ field }) => (
                            <Select
                                value={field.value}
                                onValueChange={field.onChange}
                            >
                                <SelectTrigger>
                                    <SelectValue placeholder="Selecciona prioridad" />
                                </SelectTrigger>
                                <SelectContent>
                                    <SelectItem value="Alta">Alta</SelectItem>
                                    <SelectItem value="Media">Media</SelectItem>
                                    <SelectItem value="Baja">Baja</SelectItem>
                                </SelectContent>
                            </Select>
                        )}
                    />
                    {necErrors?.prioridad && (
                        <p className="text-sm text-destructive mt-1">
                            {necErrors.prioridad.message}
                        </p>
                    )}
                </div>
            </div>
        </Card>
    )
}
