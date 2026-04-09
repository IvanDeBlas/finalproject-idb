"use client"

import { useState } from "react"
import { Button } from "@/components/ui/button"
import { Plus, Info } from "lucide-react"
import { PromoTareaCard } from "../tareas/PromoTareaCard"
import { PromoTareaForm } from "../tareas/PromoTareaForm"
import { EliminarTareaDialog } from "../tareas/EliminarTareaDialog"
import type { CreatePromoTareaItem } from "@shared/types"

interface TareasStepProps {
    tareas: CreatePromoTareaItem[]
    onTareasChange: (tareas: CreatePromoTareaItem[]) => void
    onNext: () => void
    tienePromotores?: boolean
}

export function TareasStep({ tareas, onTareasChange, tienePromotores }: TareasStepProps) {
    const [editandoTareaIndex, setEditandoTareaIndex] = useState<number | null>(null)
    const [mostrandoFormNuevo, setMostrandoFormNuevo] = useState(false)
    const [tareaAEliminarIndex, setTareaAEliminarIndex] = useState<number | null>(null)

    const handleGuardarNueva = (tarea: CreatePromoTareaItem) => {
        onTareasChange([...tareas, tarea])
        setMostrandoFormNuevo(false)
    }

    const handleGuardarEdicion = (tarea: CreatePromoTareaItem) => {
        if (editandoTareaIndex === null) return
        const nuevasTareas = [...tareas]
        nuevasTareas[editandoTareaIndex] = tarea
        onTareasChange(nuevasTareas)
        setEditandoTareaIndex(null)
    }

    const handleEliminar = () => {
        if (tareaAEliminarIndex === null) return
        const nuevasTareas = tareas.filter((_, i) => i !== tareaAEliminarIndex)
        onTareasChange(nuevasTareas)
        setTareaAEliminarIndex(null)
    }

    const handleAgregarNueva = () => {
        setEditandoTareaIndex(null)
        setMostrandoFormNuevo(true)
    }

    return (
        <div className="space-y-4 max-w-2xl mx-auto">
            {tienePromotores && (
                <div className="rounded-lg border border-amber-500/30 bg-amber-500/5 p-3">
                    <p className="text-sm text-amber-400">
                        Las tareas con completados no se pueden eliminar.
                    </p>
                </div>
            )}

            <div className="flex items-center gap-2 text-xs text-zinc-500 bg-[#1e1e38] rounded-lg p-3">
                <Info className="h-4 w-4 flex-shrink-0" />
                <span>Las tareas son opcionales. Puedes continuar sin agregar tareas.</span>
            </div>

            {tareas.map((tarea, index) => (
                editandoTareaIndex === index ? (
                    <PromoTareaForm
                        key={`edit-${index}`}
                        defaultValues={tarea}
                        onGuardar={handleGuardarEdicion}
                        onCancelar={() => setEditandoTareaIndex(null)}
                        titulo="Editar tarea"
                    />
                ) : (
                    <PromoTareaCard
                        key={`card-${index}`}
                        tarea={tarea}
                        index={index}
                        onEditar={(i) => {
                            setMostrandoFormNuevo(false)
                            setEditandoTareaIndex(i)
                        }}
                        onEliminar={(i) => setTareaAEliminarIndex(i)}
                        puedeEliminar={true}
                    />
                )
            ))}

            {mostrandoFormNuevo ? (
                <PromoTareaForm
                    onGuardar={handleGuardarNueva}
                    onCancelar={() => setMostrandoFormNuevo(false)}
                    titulo="Nueva tarea"
                />
            ) : (
                editandoTareaIndex === null && (
                    <Button
                        variant="outline"
                        className="w-full border-dashed border-zinc-700 text-zinc-400 hover:text-white"
                        onClick={handleAgregarNueva}
                    >
                        <Plus className="h-4 w-4 mr-2" />
                        Agregar nueva tarea
                    </Button>
                )
            )}

            <EliminarTareaDialog
                open={tareaAEliminarIndex !== null}
                onOpenChange={(open) => { if (!open) setTareaAEliminarIndex(null) }}
                onConfirmar={handleEliminar}
                nombreTarea={tareaAEliminarIndex !== null ? tareas[tareaAEliminarIndex]?.titulo ?? "" : ""}
            />
        </div>
    )
}
