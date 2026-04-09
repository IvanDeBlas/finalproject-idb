"use client"

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Alert, AlertDescription } from "@/components/ui/alert"
import { Plus, AlertCircle } from "lucide-react"
import { useFormContext, useFieldArray } from "react-hook-form"
import { NecesidadFormItem } from "./NecesidadFormItem"
import type { RolProfesionalConCategoria } from "@shared/types"
import type { CreateTemplateFormData } from "@/lib/validations/template.schema"

interface NecesidadesFormSectionProps {
    rolesProfesionales: RolProfesionalConCategoria[]
}

export function NecesidadesFormSection({
    rolesProfesionales,
}: NecesidadesFormSectionProps) {
    const {
        control,
        formState: { errors },
    } = useFormContext<CreateTemplateFormData>()

    const { fields, append, remove } = useFieldArray({
        control,
        name: "necesidades",
    })

    const handleAdd = () => {
        append({
            fase: "",
            titulo: "",
            descripcion: "",
            rolProfesionalId: 0,
            precioMinOrientativo: undefined,
            precioMaxOrientativo: undefined,
            prioridad: "Media",
            orden: fields.length,
        })
    }

    return (
        <Card>
            <CardHeader className="flex flex-row items-center justify-between">
                <CardTitle>Necesidades ({fields.length})</CardTitle>
                <Button type="button" variant="outline" size="sm" onClick={handleAdd}>
                    <Plus className="h-4 w-4 mr-2" />
                    Agregar necesidad
                </Button>
            </CardHeader>
            <CardContent className="space-y-3">
                {errors.necesidades?.root && (
                    <Alert variant="destructive">
                        <AlertCircle className="h-4 w-4" />
                        <AlertDescription>
                            {errors.necesidades.root.message}
                        </AlertDescription>
                    </Alert>
                )}

                {fields.length === 0 && (
                    <div className="text-center py-8 text-muted-foreground">
                        <p className="mb-2">No hay necesidades agregadas</p>
                        <p className="text-sm">
                            Haz clic en &quot;Agregar necesidad&quot; para comenzar
                        </p>
                    </div>
                )}

                {fields.map((field, index) => (
                    <NecesidadFormItem
                        key={field.id}
                        index={index}
                        rolesProfesionales={rolesProfesionales}
                        onDelete={() => remove(index)}
                    />
                ))}
            </CardContent>
        </Card>
    )
}
