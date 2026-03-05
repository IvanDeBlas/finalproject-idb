"use client"

import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from "@/components/ui/select"
import type { Control, FieldErrors, FieldValues } from "react-hook-form"
import { Controller } from "react-hook-form"
import type { MaestraMoneda } from "@shared/types"

interface PresupuestoFieldsProps {
    control: Control<FieldValues>
    errors: FieldErrors<FieldValues>
    monedas?: MaestraMoneda[]
}

export function PresupuestoFields({ control, errors, monedas }: PresupuestoFieldsProps) {
    return (
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div>
                <Label htmlFor="presupuestoMin">Presupuesto minimo</Label>
                <Controller
                    name="presupuestoMin"
                    control={control}
                    render={({ field }) => (
                        <Input
                            id="presupuestoMin"
                            type="number"
                            min={0}
                            step={1}
                            placeholder="0"
                            value={(field.value as string | number) ?? ""}
                            onChange={(e) => {
                                const val = e.target.value
                                field.onChange(val === "" ? undefined : Number(val))
                            }}
                        />
                    )}
                />
                {errors.presupuestoMin && (
                    <p className="text-sm text-red-400 mt-1">
                        {(errors.presupuestoMin as { message?: string }).message}
                    </p>
                )}
            </div>
            <div>
                <Label htmlFor="presupuestoMax">Presupuesto maximo</Label>
                <Controller
                    name="presupuestoMax"
                    control={control}
                    render={({ field }) => (
                        <Input
                            id="presupuestoMax"
                            type="number"
                            min={0}
                            step={1}
                            placeholder="0"
                            value={(field.value as string | number) ?? ""}
                            onChange={(e) => {
                                const val = e.target.value
                                field.onChange(val === "" ? undefined : Number(val))
                            }}
                        />
                    )}
                />
                {errors.presupuestoMax && (
                    <p className="text-sm text-red-400 mt-1">
                        {(errors.presupuestoMax as { message?: string }).message}
                    </p>
                )}
            </div>
            <div>
                <Label htmlFor="monedaId">Moneda</Label>
                <Controller
                    name="monedaId"
                    control={control}
                    render={({ field }) => (
                        <Select
                            value={field.value?.toString() || ""}
                            onValueChange={(val) => field.onChange(val ? Number(val) : undefined)}
                        >
                            <SelectTrigger id="monedaId">
                                <SelectValue placeholder="Selecciona moneda" />
                            </SelectTrigger>
                            <SelectContent>
                                {monedas?.map((moneda) => (
                                    <SelectItem key={moneda.id} value={moneda.id.toString()}>
                                        {moneda.nombre} ({moneda.simbolo})
                                    </SelectItem>
                                ))}
                            </SelectContent>
                        </Select>
                    )}
                />
                {errors.monedaId && (
                    <p className="text-sm text-red-400 mt-1">
                        {(errors.monedaId as { message?: string }).message}
                    </p>
                )}
            </div>
        </div>
    )
}
