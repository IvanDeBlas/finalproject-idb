"use client"

import { useSortable } from "@dnd-kit/sortable"
import { CSS } from "@dnd-kit/utilities"
import {
    GripVertical,
    Edit,
    Trash2,
    Package,
    Infinity,
    CheckCircle,
    XCircle,
} from "lucide-react"
import { Card, Button, Badge } from "@/components/ui"
import type { Reward } from "@shared/types"

interface RewardCardProps {
    reward: Reward
    onEdit: () => void
    onDelete: () => void
}

export function RewardCard({ reward, onEdit, onDelete }: RewardCardProps) {
    const {
        attributes,
        listeners,
        setNodeRef,
        transform,
        transition,
        isDragging,
    } = useSortable({ id: reward.id })

    const style = {
        transform: CSS.Transform.toString(transform),
        transition,
        opacity: isDragging ? 0.5 : 1,
    }

    const hasLimitedStock =
        reward.cantidadMaxima != null && reward.cantidadMaxima !== undefined
    const cantidadVendida = 0 // Backend doesn't expose this yet; placeholder
    const stockDisponible = hasLimitedStock
        ? reward.cantidadMaxima! - cantidadVendida
        : null
    const stockPercentage =
        hasLimitedStock && reward.cantidadMaxima
            ? (cantidadVendida / reward.cantidadMaxima) * 100
            : 0
    const isSoldOut = stockDisponible !== null && stockDisponible <= 0
    const isLowStock =
        stockDisponible !== null && stockPercentage >= 50 && !isSoldOut

    return (
        <Card
            ref={setNodeRef}
            style={style}
            className="bg-card border-border p-5 hover:border-primary cursor-move transition group relative"
        >
            {/* Drag Handle */}
            <div
                {...attributes}
                {...listeners}
                className="absolute left-2 top-1/2 -translate-y-1/2 text-muted-foreground group-hover:text-primary cursor-grab active:cursor-grabbing"
            >
                <GripVertical className="w-5 h-5" />
            </div>

            {/* Content */}
            <div className="pl-8 pr-20">
                {/* Header */}
                <div className="flex items-start justify-between mb-3">
                    <div className="flex items-baseline gap-3">
                        <span className="text-2xl font-bold text-primary">
                            &euro; {reward.importeMinimo.toFixed(2)}
                        </span>
                        <h3 className="text-lg font-semibold text-foreground">
                            {reward.nombre}
                        </h3>
                    </div>
                </div>

                {/* Description */}
                {reward.descripcion && (
                    <p className="text-sm text-muted-foreground mb-3 line-clamp-2">
                        {reward.descripcion}
                    </p>
                )}

                {/* Meta Row */}
                <div className="flex items-center gap-4 text-xs text-muted-foreground">
                    {/* Stock Indicator */}
                    <div className="flex items-center gap-1">
                        {isSoldOut ? (
                            <>
                                <XCircle className="w-4 h-4 text-muted-foreground" />
                                <span className="text-muted-foreground">
                                    Agotado
                                </span>
                            </>
                        ) : hasLimitedStock ? (
                            <>
                                <Package className="w-4 h-4 text-amber-500" />
                                <span className="text-muted-foreground">
                                    {stockDisponible} de{" "}
                                    {reward.cantidadMaxima} disponibles
                                </span>
                                {isLowStock && (
                                    <Badge
                                        variant="outline"
                                        className="border-amber-500 text-amber-500 bg-amber-500/10 ml-2"
                                    >
                                        {Math.round(stockPercentage)}% vendido
                                    </Badge>
                                )}
                            </>
                        ) : (
                            <>
                                <Infinity className="w-4 h-4 text-green-400" />
                                <span className="text-green-400">
                                    Ilimitadas disponibles
                                </span>
                            </>
                        )}
                    </div>

                    {/* Backers Count */}
                    <div className="flex items-center gap-1">
                        <CheckCircle className="w-4 h-4 text-primary" />
                        <span className="text-muted-foreground">
                            {cantidadVendida} backers
                        </span>
                    </div>
                </div>
            </div>

            {/* Actions */}
            <div className="absolute right-4 top-4 flex gap-2">
                <Button
                    variant="ghost"
                    size="sm"
                    onClick={onEdit}
                    className="text-primary hover:bg-primary/10"
                >
                    <Edit className="w-4 h-4 md:mr-2" />
                    <span className="hidden md:inline">Editar</span>
                </Button>
                <Button
                    variant="ghost"
                    size="sm"
                    onClick={onDelete}
                    className="text-red-400 hover:bg-red-500/10"
                >
                    <Trash2 className="w-4 h-4" />
                </Button>
            </div>
        </Card>
    )
}
