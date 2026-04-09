"use client"

import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import type { Control, FieldErrors, FieldValues } from "react-hook-form"
import { useController } from "react-hook-form"

interface UbicacionFieldsProps {
    control: Control<FieldValues>
    errors: FieldErrors<FieldValues>
}

export function UbicacionFields({ control, errors }: UbicacionFieldsProps) {
    const ciudad = useController({ name: "ubicacionCiudad", control })
    const pais = useController({ name: "ubicacionPais", control })

    return (
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-4">
            <div>
                <Label htmlFor="ubicacionCiudad">Ciudad *</Label>
                <Input
                    id="ubicacionCiudad"
                    value={(ciudad.field.value as string) || ""}
                    onChange={ciudad.field.onChange}
                    onBlur={ciudad.field.onBlur}
                    placeholder="Ej: Madrid"
                />
                {errors.ubicacionCiudad && (
                    <p className="text-sm text-red-400 mt-1">
                        {errors.ubicacionCiudad.message as string}
                    </p>
                )}
            </div>
            <div>
                <Label htmlFor="ubicacionPais">Pais</Label>
                <Input
                    id="ubicacionPais"
                    value={(pais.field.value as string) || ""}
                    onChange={pais.field.onChange}
                    onBlur={pais.field.onBlur}
                    placeholder="Ej: Espana"
                />
                {errors.ubicacionPais && (
                    <p className="text-sm text-red-400 mt-1">
                        {errors.ubicacionPais.message as string}
                    </p>
                )}
            </div>
        </div>
    )
}
