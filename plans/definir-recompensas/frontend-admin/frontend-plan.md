# Plan Frontend: Definir Recompensas (Admin Dashboard)

**Fecha:** 2026-02-13
**Feature:** definir-recompensas
**Target:** src/admin (Next.js 14 Dashboard)

## 1. Resumen

- Screens: 1 (RewardsPage + modal/dialog)
- Componentes: 4 (RewardsListPage, RewardCard, RewardFormModal, RewardDeleteDialog)
- Hooks: 5 (useRewards, useCreateReward, useUpdateReward, useDeleteReward, useReorderRewards)
- Services: 1 (reward.service.ts)

## 2. Estructura de Carpetas

```
src/admin/src/
├── app/
│   └── (dashboard)/
│       └── campanias/
│           └── [id]/
│               └── recompensas/
│                   ├── page.tsx                      # RewardsListPage (main page)
│                   └── components/
│                       ├── RewardCard.tsx            # Sortable reward card
│                       ├── RewardFormModal.tsx       # Create/Edit modal
│                       ├── RewardDeleteDialog.tsx    # Delete confirmation
│                       └── RewardStatsCard.tsx       # Stats summary
│
├── hooks/
│   └── use-rewards.ts                                # All reward hooks
│
├── services/
│   └── reward.service.ts                             # API calls
│
└── components/
    └── ui/
        ├── dialog.tsx                                # shadcn (already exists)
        ├── alert-dialog.tsx                          # shadcn (needs creation)
        ├── checkbox.tsx                              # shadcn (needs creation)
        └── ...
```

## 3. Componentes

### 3.1 RewardsListPage

**Archivo:** `src/admin/src/app/(dashboard)/campanias/[id]/recompensas/page.tsx`

**Tipo:** Next.js Page Component

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| params | { id: string } | Si | Route params (campaniaId) |

**Estado Local:**
- `selectedReward: Reward | null` - Reward seleccionada para edit
- `rewardToDelete: Reward | null` - Reward seleccionada para delete
- `isFormModalOpen: boolean` - Estado del modal de formulario
- `isDeleteDialogOpen: boolean` - Estado del dialog de eliminacion

**Dependencias:**
- Hooks: `useRewards()`, `useReorderRewards()`
- Componentes UI: `Card`, `Button`, `Badge`
- Componentes locales: `RewardCard`, `RewardFormModal`, `RewardDeleteDialog`, `RewardStatsCard`
- External: `@dnd-kit/core`, `@dnd-kit/sortable`, `lucide-react` (icons)

**Responsabilidad:**
Pagina principal de gestion de recompensas. Muestra lista sortable de reward cards con drag & drop, stats card, botones de accion (crear, editar, eliminar), y maneja el estado de modals/dialogs. Incluye empty state cuando no hay recompensas.

**Estructura interna:**

```tsx
"use client"

import { useState } from "react"
import { useParams, useRouter } from "next/navigation"
import { DndContext, DragEndEvent, closestCenter } from "@dnd-kit/core"
import { SortableContext, verticalListSortingStrategy, arrayMove } from "@dnd-kit/sortable"
import { Plus, Lightbulb, ArrowLeft } from "lucide-react"
import { Card, Button, Badge } from "@/components/ui"
import { useRewards, useReorderRewards } from "@/hooks/use-rewards"
import { RewardCard } from "./components/RewardCard"
import { RewardFormModal } from "./components/RewardFormModal"
import { RewardDeleteDialog } from "./components/RewardDeleteDialog"
import { RewardStatsCard } from "./components/RewardStatsCard"
import { EmptyState } from "@/app/(dashboard)/campanias/components/shared/EmptyState"
import { toast } from "sonner"
import type { Reward } from "@shared/types"

export default function RewardsPage() {
    const params = useParams()
    const router = useRouter()
    const campaniaId = params.id as string

    const { data: rewards = [], isLoading } = useRewards(campaniaId)
    const { mutate: reorder, isPending: isReordering } = useReorderRewards()

    const [selectedReward, setSelectedReward] = useState<Reward | null>(null)
    const [rewardToDelete, setRewardToDelete] = useState<Reward | null>(null)
    const [isFormModalOpen, setIsFormModalOpen] = useState(false)
    const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false)

    const handleDragEnd = (event: DragEndEvent) => {
        const { active, over } = event
        if (!over || active.id === over.id) return

        const oldIndex = rewards.findIndex((r) => r.id === active.id)
        const newIndex = rewards.findIndex((r) => r.id === over.id)
        const reordered = arrayMove(rewards, oldIndex, newIndex)

        const rewardOrders = reordered.map((r, idx) => ({
            rewardId: r.id,
            orden: idx + 1,
        }))

        reorder(
            { campaniaId, rewardOrders },
            {
                onSuccess: () => {
                    toast.success("Orden actualizado")
                },
                onError: () => {
                    toast.error("Error al reordenar recompensas")
                },
            }
        )
    }

    const handleCreateClick = () => {
        setSelectedReward(null)
        setIsFormModalOpen(true)
    }

    const handleEditClick = (reward: Reward) => {
        setSelectedReward(reward)
        setIsFormModalOpen(true)
    }

    const handleDeleteClick = (reward: Reward) => {
        setRewardToDelete(reward)
        setIsDeleteDialogOpen(true)
    }

    return (
        <div className="container max-w-5xl mx-auto py-8 px-4 md:px-6">
            {/* Header */}
            <div className="mb-6">
                <Button
                    variant="ghost"
                    onClick={() => router.push(`/dashboard/campanias/${campaniaId}`)}
                    className="flex items-center gap-2 text-muted-foreground hover:text-foreground mb-4 transition"
                >
                    <ArrowLeft className="w-4 h-4" />
                    Volver a Campania
                </Button>
                <h1 className="text-2xl md:text-3xl font-bold text-foreground">
                    Recompensas
                </h1>
                <p className="text-sm text-muted-foreground mt-1">
                    Gestiona las recompensas de tu campania
                </p>
            </div>

            {/* Stats Card */}
            {rewards.length > 0 && <RewardStatsCard rewards={rewards} />}

            {/* Empty State */}
            {!isLoading && rewards.length === 0 && (
                <EmptyState
                    icon={<Package className="w-20 h-20" />}
                    title="No has creado recompensas aun"
                    description="Las recompensas incentivan a tus fans a apoyarte. Crea tu primera recompensa ahora."
                    action={
                        <Button
                            onClick={handleCreateClick}
                            className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold"
                        >
                            <Plus className="w-5 h-5 mr-2" />
                            Crear primera recompensa
                        </Button>
                    }
                />
            )}

            {/* Rewards List (DnD) */}
            {rewards.length > 0 && (
                <>
                    <DndContext
                        collisionDetection={closestCenter}
                        onDragEnd={handleDragEnd}
                    >
                        <SortableContext
                            items={rewards.map((r) => r.id)}
                            strategy={verticalListSortingStrategy}
                        >
                            <div className="space-y-3 mb-4">
                                {rewards.map((reward) => (
                                    <RewardCard
                                        key={reward.id}
                                        reward={reward}
                                        onEdit={() => handleEditClick(reward)}
                                        onDelete={() => handleDeleteClick(reward)}
                                    />
                                ))}
                            </div>
                        </SortableContext>
                    </DndContext>

                    {/* Add Reward Button */}
                    <Button
                        onClick={handleCreateClick}
                        variant="outline"
                        className="w-full border-dashed border-primary text-primary hover:bg-primary/10 py-6 mb-4"
                    >
                        <Plus className="w-5 h-5 mr-2" />
                        Agregar recompensa
                    </Button>
                </>
            )}

            {/* Tip Box */}
            {rewards.length > 0 && (
                <div className="bg-card border-l-4 border-primary p-4 rounded-r-lg">
                    <div className="flex items-start gap-3">
                        <Lightbulb className="w-5 h-5 text-primary mt-0.5" />
                        <div>
                            <p className="text-sm text-muted-foreground">
                                <strong>Tip:</strong> Ordena tus recompensas arrastrándolas.
                                El orden se reflejará en la vista pública.
                            </p>
                        </div>
                    </div>
                </div>
            )}

            {/* Modals */}
            <RewardFormModal
                isOpen={isFormModalOpen}
                onClose={() => setIsFormModalOpen(false)}
                campaniaId={campaniaId}
                reward={selectedReward}
            />

            <RewardDeleteDialog
                isOpen={isDeleteDialogOpen}
                onClose={() => setIsDeleteDialogOpen(false)}
                reward={rewardToDelete}
            />
        </div>
    )
}
```

**Estados de UI:**
- Loading: Skeleton loaders para reward cards
- Empty: EmptyState con icono, titulo, descripcion, CTA
- Default: Lista de rewards con drag handles, stats card
- Dragging: Card con opacity 0.5, drop zone indicator
- Error: Toast notification

---

### 3.2 RewardCard

**Archivo:** `src/admin/src/app/(dashboard)/campanias/[id]/recompensas/components/RewardCard.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| reward | Reward | Si | Datos de la recompensa |
| onEdit | () => void | Si | Callback para editar |
| onDelete | () => void | Si | Callback para eliminar |

**Estado Local:**
- Ninguno (componente presentacional con drag)

**Dependencias:**
- Hooks: `useSortable` de `@dnd-kit/sortable`
- Componentes UI: `Card`, `Button`, `Badge`
- Icons: `GripVertical`, `Edit`, `Trash2`, `Package`, `Infinity`, `CheckCircle`

**Responsabilidad:**
Card individual de recompensa con drag handle, informacion de precio/titulo/descripcion, indicador de stock, backers count, y botones de editar/eliminar. Usa `useSortable` para habilitar drag & drop.

**Estructura interna:**

```tsx
"use client"

import { useSortable } from "@dnd-kit/sortable"
import { CSS } from "@dnd-kit/utilities"
import { GripVertical, Edit, Trash2, Package, Infinity, CheckCircle, XCircle, AlertCircle } from "lucide-react"
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

    // Calcular stock disponible
    const hasLimitedStock = reward.cantidadMaxima !== null && reward.cantidadMaxima !== undefined
    const stockDisponible = hasLimitedStock
        ? (reward.cantidadMaxima! - (reward.cantidadVendida || 0))
        : null
    const stockPercentage = hasLimitedStock && reward.cantidadMaxima
        ? ((reward.cantidadMaxima - (stockDisponible || 0)) / reward.cantidadMaxima) * 100
        : 0
    const isSoldOut = stockDisponible !== null && stockDisponible <= 0
    const isLowStock = stockDisponible !== null && stockPercentage >= 50 && !isSoldOut

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
                            € {reward.importeMinimo.toFixed(2)}
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
                                <span className="text-muted-foreground">Agotado</span>
                            </>
                        ) : hasLimitedStock ? (
                            <>
                                <Package className="w-4 h-4 text-warning" />
                                <span className="text-muted-foreground">
                                    {stockDisponible} de {reward.cantidadMaxima} disponibles
                                </span>
                                {isLowStock && (
                                    <Badge
                                        variant="outline"
                                        className="border-warning text-warning bg-warning/10 ml-2"
                                    >
                                        {Math.round(stockPercentage)}% vendido
                                    </Badge>
                                )}
                            </>
                        ) : (
                            <>
                                <Infinity className="w-4 h-4 text-green-400" />
                                <span className="text-green-400">Ilimitadas disponibles</span>
                            </>
                        )}
                    </div>

                    {/* Backers Count */}
                    <div className="flex items-center gap-1">
                        <CheckCircle className="w-4 h-4 text-primary" />
                        <span className="text-muted-foreground">
                            {reward.cantidadVendida || 0} backers
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
```

**Estados de UI:**
- Default: Border normal, drag handle visible al hover (desktop)
- Hover: Border primary, elevation increase
- Dragging: Opacity 0.5, cursor grabbing
- Sold Out: Icono X, texto "Agotado"
- Low Stock: Badge warning con porcentaje vendido

---

### 3.3 RewardFormModal

**Archivo:** `src/admin/src/app/(dashboard)/campanias/[id]/recompensas/components/RewardFormModal.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| isOpen | boolean | Si | Estado del modal |
| onClose | () => void | Si | Callback para cerrar |
| campaniaId | string | Si | ID de la campania |
| reward | Reward \| null | No | Reward para editar (null = crear) |

**Estado Local:**
- `stockLimitado: boolean` - Checkbox stock limitado
- `envioFisico: boolean` - Checkbox envio fisico
- `characterCounts: { nombre: number, descripcion: number }` - Contadores de caracteres

**Dependencias:**
- Hooks: `useCreateReward()`, `useUpdateReward()`, `useForm` de react-hook-form, `zodResolver`
- Schemas: `createRewardSchema` de @shared/schemas
- Componentes UI: `Dialog`, `DialogContent`, `DialogHeader`, `DialogTitle`, `DialogFooter`, `Input`, `Textarea`, `Select`, `Checkbox`, `Button`, `Label`
- Icons: `X`, `Loader2`

**Responsabilidad:**
Modal para crear o editar recompensa con formulario completo validado por Zod. Maneja campos condicionales (stock limitado, envio fisico), character counters, validacion en tiempo real, loading states, y toast notifications.

**Estructura interna:**

```tsx
"use client"

import { useEffect, useState } from "react"
import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import { X, Loader2 } from "lucide-react"
import {
    Dialog,
    DialogContent,
    DialogHeader,
    DialogTitle,
    DialogFooter,
} from "@/components/ui/dialog"
import {
    Input,
    Textarea,
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
    Checkbox,
    Button,
    Label,
} from "@/components/ui"
import { useCreateReward, useUpdateReward } from "@/hooks/use-rewards"
import { createRewardSchema, type CreateRewardFormData } from "@shared/schemas"
import { TIPO_REWARD, TIPO_REWARD_LABELS } from "@shared/constants"
import { toast } from "sonner"
import type { Reward } from "@shared/types"

interface RewardFormModalProps {
    isOpen: boolean
    onClose: () => void
    campaniaId: string
    reward: Reward | null
}

export function RewardFormModal({
    isOpen,
    onClose,
    campaniaId,
    reward,
}: RewardFormModalProps) {
    const isEdit = !!reward
    const { mutate: create, isPending: isCreating } = useCreateReward()
    const { mutate: update, isPending: isUpdating } = useUpdateReward()
    const isPending = isCreating || isUpdating

    const [stockLimitado, setStockLimitado] = useState(false)
    const [envioFisico, setEnvioFisico] = useState(false)

    const {
        register,
        handleSubmit,
        formState: { errors },
        watch,
        reset,
        setValue,
    } = useForm<CreateRewardFormData>({
        resolver: zodResolver(createRewardSchema),
        defaultValues: {
            campaniaId,
            monedaId: 1, // EUR
            esAddOn: false,
            incluyeEnvioFisico: false,
            orden: 0,
        },
    })

    const nombreValue = watch("nombre") || ""
    const descripcionValue = watch("descripcion") || ""

    // Reset form cuando cambia reward o se abre modal
    useEffect(() => {
        if (isOpen && reward) {
            reset({
                campaniaId: reward.campaniaId,
                tipoRewardId: reward.tipoRewardId,
                nombre: reward.nombre,
                descripcion: reward.descripcion || "",
                importeMinimo: reward.importeMinimo,
                monedaId: reward.monedaId,
                esAddOn: reward.esAddOn,
                cantidadMaxima: reward.cantidadMaxima || undefined,
                cantidadPorBacker: reward.cantidadPorBacker || undefined,
                incluyeEnvioFisico: reward.incluyeEnvioFisico,
                tiempoEntregaEstimado: reward.tiempoEntregaEstimado || "",
                orden: reward.orden,
            })
            setStockLimitado(!!reward.cantidadMaxima)
            setEnvioFisico(reward.incluyeEnvioFisico)
        } else if (isOpen) {
            reset({
                campaniaId,
                monedaId: 1,
                esAddOn: false,
                incluyeEnvioFisico: false,
                orden: 0,
            })
            setStockLimitado(false)
            setEnvioFisico(false)
        }
    }, [isOpen, reward, reset, campaniaId])

    const onSubmit = (data: CreateRewardFormData) => {
        if (isEdit && reward) {
            update(
                {
                    id: reward.id,
                    data: {
                        id: reward.id,
                        ...data,
                    },
                },
                {
                    onSuccess: () => {
                        toast.success("Recompensa actualizada exitosamente")
                        onClose()
                    },
                    onError: (error: any) => {
                        toast.error(
                            error?.message || "Error al actualizar recompensa"
                        )
                    },
                }
            )
        } else {
            create(data, {
                onSuccess: () => {
                    toast.success("Recompensa creada exitosamente")
                    onClose()
                },
                onError: (error: any) => {
                    toast.error(error?.message || "Error al crear recompensa")
                },
            })
        }
    }

    return (
        <Dialog open={isOpen} onOpenChange={onClose}>
            <DialogContent className="bg-card border-border max-w-2xl max-h-[90vh] overflow-y-auto">
                <DialogHeader className="border-b border-border pb-4 mb-6">
                    <DialogTitle className="text-2xl font-bold">
                        {isEdit ? "Editar Recompensa" : "Nueva Recompensa"}
                    </DialogTitle>
                    <Button
                        variant="ghost"
                        size="sm"
                        onClick={onClose}
                        className="absolute right-4 top-4 text-muted-foreground hover:text-foreground"
                    >
                        <X className="w-5 h-5" />
                    </Button>
                </DialogHeader>

                <form onSubmit={handleSubmit(onSubmit)} className="space-y-6 p-6">
                    {/* Nombre */}
                    <div>
                        <Label htmlFor="nombre" className="text-sm font-medium text-foreground mb-2 block after:content-['*'] after:text-red-500 after:ml-1">
                            Nombre de la recompensa
                        </Label>
                        <Input
                            id="nombre"
                            {...register("nombre")}
                            placeholder="Ej: Descarga Digital"
                            className="bg-background border-border text-foreground placeholder:text-muted-foreground focus:border-primary"
                        />
                        <div className="flex items-center justify-between mt-1">
                            {errors.nombre && (
                                <p className="text-xs text-red-500">{errors.nombre.message}</p>
                            )}
                            <p className={`text-xs ml-auto ${nombreValue.length > 160 ? 'text-warning' : nombreValue.length > 190 ? 'text-red-500' : 'text-muted-foreground'}`}>
                                {nombreValue.length}/200 caracteres
                            </p>
                        </div>
                        <p className="text-xs text-muted-foreground mt-1 italic">
                            Máx 200 caracteres - Sé claro y conciso
                        </p>
                    </div>

                    {/* Descripcion */}
                    <div>
                        <Label htmlFor="descripcion" className="text-sm font-medium text-foreground mb-2 block">
                            Descripción
                        </Label>
                        <Textarea
                            id="descripcion"
                            {...register("descripcion")}
                            placeholder="Describe qué incluye esta recompensa..."
                            className="bg-background border-border text-foreground min-h-[120px] resize-none placeholder:text-muted-foreground"
                        />
                        <p className={`text-xs mt-1 ${descripcionValue.length > 1600 ? 'text-warning' : descripcionValue.length > 1900 ? 'text-red-500' : 'text-muted-foreground'}`}>
                            {descripcionValue.length}/2000 caracteres
                        </p>
                        {errors.descripcion && (
                            <p className="text-xs text-red-500 mt-1">{errors.descripcion.message}</p>
                        )}
                    </div>

                    {/* Grid 2 columns */}
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                        {/* Importe Minimo */}
                        <div>
                            <Label htmlFor="importeMinimo" className="text-sm font-medium text-foreground mb-2 block after:content-['*'] after:text-red-500 after:ml-1">
                                Importe mínimo
                            </Label>
                            <div className="relative">
                                <span className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground">€</span>
                                <Input
                                    id="importeMinimo"
                                    type="number"
                                    step="0.01"
                                    {...register("importeMinimo", { valueAsNumber: true })}
                                    placeholder="10.00"
                                    className="bg-background border-border text-foreground pl-8"
                                />
                            </div>
                            {errors.importeMinimo && (
                                <p className="text-xs text-red-500 mt-1">{errors.importeMinimo.message}</p>
                            )}
                        </div>

                        {/* Tipo Recompensa */}
                        <div>
                            <Label htmlFor="tipoRewardId" className="text-sm font-medium text-foreground mb-2 block after:content-['*'] after:text-red-500 after:ml-1">
                                Tipo de recompensa
                            </Label>
                            <Select onValueChange={(value) => setValue("tipoRewardId", Number(value))}>
                                <SelectTrigger className="bg-background border-border text-foreground">
                                    <SelectValue placeholder="Seleccionar tipo" />
                                </SelectTrigger>
                                <SelectContent>
                                    {Object.entries(TIPO_REWARD_LABELS).map(([id, label]) => (
                                        <SelectItem key={id} value={id}>
                                            {label}
                                        </SelectItem>
                                    ))}
                                </SelectContent>
                            </Select>
                            {errors.tipoRewardId && (
                                <p className="text-xs text-red-500 mt-1">{errors.tipoRewardId.message}</p>
                            )}
                        </div>
                    </div>

                    {/* Es Add-on */}
                    <div className="flex items-center gap-2">
                        <Checkbox
                            id="esAddOn"
                            {...register("esAddOn")}
                            className="border-border data-[state=checked]:bg-primary data-[state=checked]:border-primary"
                        />
                        <Label htmlFor="esAddOn" className="text-sm text-foreground cursor-pointer">
                            Es add-on (complemento)
                        </Label>
                    </div>

                    {/* Stock Limitado */}
                    <div>
                        <div className="flex items-center gap-2 mb-2">
                            <Checkbox
                                id="stockLimitado"
                                checked={stockLimitado}
                                onCheckedChange={(checked) => {
                                    setStockLimitado(checked as boolean)
                                    if (!checked) {
                                        setValue("cantidadMaxima", undefined)
                                        setValue("cantidadPorBacker", undefined)
                                    }
                                }}
                                className="border-border data-[state=checked]:bg-primary data-[state=checked]:border-primary"
                            />
                            <Label htmlFor="stockLimitado" className="text-sm text-foreground cursor-pointer">
                                Stock limitado
                            </Label>
                        </div>

                        {stockLimitado && (
                            <div className="bg-background border border-border p-4 rounded-lg mt-2 space-y-4">
                                <div>
                                    <Label htmlFor="cantidadMaxima" className="text-sm font-medium text-foreground mb-2 block">
                                        Cantidad máxima disponible
                                    </Label>
                                    <Input
                                        id="cantidadMaxima"
                                        type="number"
                                        {...register("cantidadMaxima", { valueAsNumber: true })}
                                        placeholder="200"
                                        className="bg-card border-border text-foreground"
                                    />
                                    {errors.cantidadMaxima && (
                                        <p className="text-xs text-red-500 mt-1">{errors.cantidadMaxima.message}</p>
                                    )}
                                </div>

                                <div>
                                    <Label htmlFor="cantidadPorBacker" className="text-sm font-medium text-foreground mb-2 block">
                                        Máximo por backer (opcional)
                                    </Label>
                                    <Input
                                        id="cantidadPorBacker"
                                        type="number"
                                        {...register("cantidadPorBacker", { valueAsNumber: true })}
                                        placeholder="1"
                                        className="bg-card border-border text-foreground"
                                    />
                                    {errors.cantidadPorBacker && (
                                        <p className="text-xs text-red-500 mt-1">{errors.cantidadPorBacker.message}</p>
                                    )}
                                </div>
                            </div>
                        )}
                    </div>

                    {/* Envio Fisico */}
                    <div>
                        <div className="flex items-center gap-2 mb-2">
                            <Checkbox
                                id="incluyeEnvioFisico"
                                checked={envioFisico}
                                onCheckedChange={(checked) => {
                                    setEnvioFisico(checked as boolean)
                                    setValue("incluyeEnvioFisico", checked as boolean)
                                    if (!checked) {
                                        setValue("tiempoEntregaEstimado", "")
                                    }
                                }}
                                className="border-border data-[state=checked]:bg-primary data-[state=checked]:border-primary"
                            />
                            <Label htmlFor="incluyeEnvioFisico" className="text-sm text-foreground cursor-pointer">
                                Incluye envío físico
                            </Label>
                        </div>

                        {envioFisico && (
                            <div className="bg-background border border-border p-4 rounded-lg mt-2">
                                <Label htmlFor="tiempoEntregaEstimado" className="text-sm font-medium text-foreground mb-2 block">
                                    Tiempo de entrega estimado
                                </Label>
                                <Input
                                    id="tiempoEntregaEstimado"
                                    {...register("tiempoEntregaEstimado")}
                                    placeholder="Marzo 2025"
                                    className="bg-card border-border text-foreground"
                                />
                                {errors.tiempoEntregaEstimado && (
                                    <p className="text-xs text-red-500 mt-1">{errors.tiempoEntregaEstimado.message}</p>
                                )}
                            </div>
                        )}
                    </div>

                    {/* Footer */}
                    <DialogFooter className="border-t border-border pt-4 mt-6 flex gap-3 justify-end">
                        <Button
                            type="button"
                            variant="outline"
                            onClick={onClose}
                            disabled={isPending}
                            className="border-border text-muted-foreground hover:text-foreground hover:bg-secondary"
                        >
                            Cancelar
                        </Button>
                        <Button
                            type="submit"
                            disabled={isPending}
                            className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold px-8"
                        >
                            {isPending && <Loader2 className="w-4 h-4 mr-2 animate-spin" />}
                            {isPending ? "Guardando..." : "Guardar"}
                        </Button>
                    </DialogFooter>
                </form>
            </DialogContent>
        </Dialog>
    )
}
```

**Estados de UI:**
- Default (Create): Form vacio, titulo "Nueva Recompensa"
- Default (Edit): Form pre-completado, titulo "Editar Recompensa"
- Focus Input: Border primary, glow shadow
- Typing: Character counters actualizados, warning color a 80%
- Conditional sections: Fade-in animation cuando checkbox checked
- Validation Error: Input border red, mensaje error debajo
- Loading: Spinner en boton, inputs disabled
- Success: Toast verde, modal cierra

---

### 3.4 RewardDeleteDialog

**Archivo:** `src/admin/src/app/(dashboard)/campanias/[id]/recompensas/components/RewardDeleteDialog.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| isOpen | boolean | Si | Estado del dialog |
| onClose | () => void | Si | Callback para cerrar |
| reward | Reward \| null | No | Reward a eliminar |

**Estado Local:**
- Ninguno (componente controlado por props)

**Dependencias:**
- Hooks: `useDeleteReward()`
- Componentes UI: `AlertDialog`, `AlertDialogContent`, `AlertDialogHeader`, `AlertDialogTitle`, `AlertDialogDescription`, `AlertDialogFooter`, `AlertDialogCancel`, `AlertDialogAction`, `Button`
- Icons: `AlertTriangle`, `Loader2`

**Responsabilidad:**
Dialog de confirmacion para eliminar recompensa. Muestra dos variantes: sin backings (eliminar directo) y con backings (desactivar). Maneja loading state y toast notifications.

**Estructura interna:**

```tsx
"use client"

import { AlertTriangle, Loader2 } from "lucide-react"
import {
    AlertDialog,
    AlertDialogContent,
    AlertDialogHeader,
    AlertDialogTitle,
    AlertDialogDescription,
    AlertDialogFooter,
    AlertDialogCancel,
    AlertDialogAction,
} from "@/components/ui/alert-dialog"
import { useDeleteReward } from "@/hooks/use-rewards"
import { toast } from "sonner"
import type { Reward } from "@shared/types"

interface RewardDeleteDialogProps {
    isOpen: boolean
    onClose: () => void
    reward: Reward | null
}

export function RewardDeleteDialog({
    isOpen,
    onClose,
    reward,
}: RewardDeleteDialogProps) {
    const { mutate: deleteReward, isPending } = useDeleteReward()

    if (!reward) return null

    const hasBackings = (reward.cantidadVendida || 0) > 0

    const handleDelete = () => {
        deleteReward(reward.id, {
            onSuccess: () => {
                toast.success("Recompensa eliminada exitosamente")
                onClose()
            },
            onError: (error: any) => {
                if (error?.errorCode === "4010") {
                    toast.error(
                        "Esta recompensa tiene aportes y no puede ser eliminada. Desactívala en su lugar."
                    )
                } else {
                    toast.error(error?.message || "Error al eliminar recompensa")
                }
            },
        })
    }

    const handleDeactivate = () => {
        // TODO: Implementar endpoint de desactivacion PUT /api/rewards/{id}/deactivate
        // Por ahora, usar update con esActivo = false
        toast.info("Funcionalidad de desactivar en desarrollo")
        onClose()
    }

    return (
        <AlertDialog open={isOpen} onOpenChange={onClose}>
            <AlertDialogContent className="bg-card border-border max-w-md">
                <AlertDialogHeader className="border-b border-border pb-4 mb-4">
                    <AlertDialogTitle className="flex items-center gap-2 text-xl font-bold text-foreground">
                        <AlertTriangle className="w-6 h-6 text-warning" />
                        {hasBackings ? "No se puede eliminar" : "Eliminar Recompensa"}
                    </AlertDialogTitle>
                </AlertDialogHeader>

                <AlertDialogDescription className="text-muted-foreground space-y-3">
                    {hasBackings ? (
                        <>
                            <p>
                                Esta recompensa no se puede eliminar porque tiene{" "}
                                <strong>{reward.cantidadVendida} backings confirmados</strong>.
                            </p>
                            <p>
                                Puedes desactivarla para que no aparezca a nuevos backers,
                                pero los existentes mantendrán su selección.
                            </p>
                        </>
                    ) : (
                        <>
                            <p>¿Estás seguro que deseas eliminar esta recompensa?</p>
                            <p className="font-semibold text-foreground bg-background px-3 py-2 rounded border border-border">
                                {reward.nombre} (€{reward.importeMinimo.toFixed(2)})
                            </p>
                            <p className="text-sm text-muted-foreground italic">
                                Esta acción no se puede deshacer.
                            </p>
                        </>
                    )}
                </AlertDialogDescription>

                <AlertDialogFooter className="border-t border-border pt-4 mt-6 flex gap-3 justify-end">
                    <AlertDialogCancel
                        disabled={isPending}
                        className="border-border text-muted-foreground hover:text-foreground hover:bg-secondary"
                    >
                        Cancelar
                    </AlertDialogCancel>
                    {hasBackings ? (
                        <AlertDialogAction
                            onClick={handleDeactivate}
                            disabled={isPending}
                            className="bg-warning hover:bg-warning/90 text-card font-semibold"
                        >
                            {isPending && <Loader2 className="w-4 h-4 mr-2 animate-spin" />}
                            {isPending ? "Desactivando..." : "Desactivar"}
                        </AlertDialogAction>
                    ) : (
                        <AlertDialogAction
                            onClick={handleDelete}
                            disabled={isPending}
                            className="bg-red-500 hover:bg-red-600 text-white font-semibold"
                        >
                            {isPending && <Loader2 className="w-4 h-4 mr-2 animate-spin" />}
                            {isPending ? "Eliminando..." : "Eliminar"}
                        </AlertDialogAction>
                    )}
                </AlertDialogFooter>
            </AlertDialogContent>
        </AlertDialog>
    )
}
```

**Estados de UI:**
- Variante 1 (sin backings): Titulo "Eliminar Recompensa", boton rojo "Eliminar"
- Variante 2 (con backings): Titulo "No se puede eliminar", boton amarillo "Desactivar"
- Loading: Spinner en boton, botones disabled
- Success: Toast, dialog cierra
- Error: Toast error (especialmente 4010)

---

### 3.5 RewardStatsCard

**Archivo:** `src/admin/src/app/(dashboard)/campanias/[id]/recompensas/components/RewardStatsCard.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| rewards | Reward[] | Si | Lista de recompensas |

**Estado Local:**
- Ninguno (calcula stats desde props)

**Dependencias:**
- Componentes UI: `Card`
- Icons: `Package`, `CheckCircle`, `TrendingUp`

**Responsabilidad:**
Card de estadisticas que muestra total de recompensas, activas, y stock total disponible. Calcula dinamicamente desde la lista de rewards.

**Estructura interna:**

```tsx
"use client"

import { Package, CheckCircle, TrendingUp } from "lucide-react"
import { Card } from "@/components/ui"
import type { Reward } from "@shared/types"

interface RewardStatsCardProps {
    rewards: Reward[]
}

export function RewardStatsCard({ rewards }: RewardStatsCardProps) {
    const total = rewards.length
    const activas = rewards.filter((r) => r.esActivo).length
    const stockTotal = rewards.reduce((sum, r) => {
        if (r.cantidadMaxima !== null && r.cantidadMaxima !== undefined) {
            return sum + (r.cantidadMaxima - (r.cantidadVendida || 0))
        }
        return sum
    }, 0)

    return (
        <Card className="bg-card border-border p-4 mb-6">
            <div className="flex items-center gap-6 text-sm">
                <div className="flex items-center gap-2">
                    <Package className="w-4 h-4 text-primary" />
                    <span className="text-muted-foreground">Total:</span>
                    <span className="font-semibold text-foreground">{total}</span>
                </div>
                <div className="flex items-center gap-2">
                    <CheckCircle className="w-4 h-4 text-primary" />
                    <span className="text-muted-foreground">Activas:</span>
                    <span className="font-semibold text-foreground">{activas}</span>
                </div>
                <div className="flex items-center gap-2">
                    <TrendingUp className="w-4 h-4 text-primary" />
                    <span className="text-muted-foreground">Stock disponible:</span>
                    <span className="font-semibold text-foreground">
                        {stockTotal > 0 ? `${stockTotal} unidades` : "Ilimitado"}
                    </span>
                </div>
            </div>
        </Card>
    )
}
```

---

## 4. Hooks

### 4.1 useRewards

**Archivo:** `src/admin/src/hooks/use-rewards.ts`

**Tipo:** Query Hook

**Parametros:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| campaniaId | string | ID de la campania |

**Retorna:**
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| data | Reward[] | Lista de recompensas |
| isLoading | boolean | Estado de carga |
| error | Error | Error si hay |

**Query Key:** `QUERY_KEYS.rewards.byCampania(campaniaId)`

**Implementacion:**

```typescript
export function useRewards(campaniaId: string) {
    return useQuery({
        queryKey: QUERY_KEYS.rewards.byCampania(campaniaId),
        queryFn: () => rewardService.getByCampania(campaniaId),
        enabled: !!campaniaId,
    })
}
```

---

### 4.2 useCreateReward

**Archivo:** `src/admin/src/hooks/use-rewards.ts`

**Tipo:** Mutation Hook

**Acciones:**
- `mutate(data: CreateRewardRequest)` - Ejecutar creacion
- `onSuccess` - Invalidar queries rewards.byCampania, toast success
- `onError` - Toast error

**Implementacion:**

```typescript
export function useCreateReward() {
    const queryClient = useQueryClient()

    return useMutation({
        mutationFn: (data: CreateRewardRequest) => rewardService.create(data),
        onSuccess: (_, variables) => {
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.rewards.byCampania(variables.campaniaId),
            })
        },
    })
}
```

---

### 4.3 useUpdateReward

**Archivo:** `src/admin/src/hooks/use-rewards.ts`

**Tipo:** Mutation Hook

**Acciones:**
- `mutate({ id, data })` - Ejecutar actualizacion
- `onSuccess` - Invalidar queries rewards.byCampania y rewards.byId, toast success
- `onError` - Toast error

**Implementacion:**

```typescript
export function useUpdateReward() {
    const queryClient = useQueryClient()

    return useMutation({
        mutationFn: ({ id, data }: { id: string; data: UpdateRewardRequest }) =>
            rewardService.update(id, data),
        onSuccess: (reward) => {
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.rewards.byCampania(reward.campaniaId),
            })
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.rewards.byId(reward.id),
            })
        },
    })
}
```

---

### 4.4 useDeleteReward

**Archivo:** `src/admin/src/hooks/use-rewards.ts`

**Tipo:** Mutation Hook

**Acciones:**
- `mutate(id: string)` - Ejecutar eliminacion
- `onSuccess` - Invalidar queries rewards.byCampania, toast success
- `onError` - Toast error (especialmente 4010)

**Implementacion:**

```typescript
export function useDeleteReward() {
    const queryClient = useQueryClient()

    return useMutation({
        mutationFn: (id: string) => rewardService.delete(id),
        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.rewards.all,
            })
        },
    })
}
```

---

### 4.5 useReorderRewards

**Archivo:** `src/admin/src/hooks/use-rewards.ts`

**Tipo:** Mutation Hook

**Acciones:**
- `mutate({ campaniaId, rewardOrders })` - Ejecutar reordenamiento
- `onSuccess` - Invalidar queries rewards.byCampania, toast success
- `onError` - Toast error

**Implementacion:**

```typescript
export function useReorderRewards() {
    const queryClient = useQueryClient()

    return useMutation({
        mutationFn: (data: ReorderRewardsRequest) => rewardService.reorder(data),
        onSuccess: (_, variables) => {
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.rewards.byCampania(variables.campaniaId),
            })
        },
    })
}
```

---

### Archivo Completo: use-rewards.ts

```typescript
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query"
import { rewardService } from "@/services/reward.service"
import { QUERY_KEYS } from "@shared/constants"
import type {
    CreateRewardRequest,
    UpdateRewardRequest,
    ReorderRewardsRequest,
} from "@shared/types"

export function useRewards(campaniaId: string) {
    return useQuery({
        queryKey: QUERY_KEYS.rewards.byCampania(campaniaId),
        queryFn: () => rewardService.getByCampania(campaniaId),
        enabled: !!campaniaId,
    })
}

export function useReward(id: string) {
    return useQuery({
        queryKey: QUERY_KEYS.rewards.byId(id),
        queryFn: () => rewardService.getById(id),
        enabled: !!id,
    })
}

export function useCreateReward() {
    const queryClient = useQueryClient()

    return useMutation({
        mutationFn: (data: CreateRewardRequest) => rewardService.create(data),
        onSuccess: (_, variables) => {
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.rewards.byCampania(variables.campaniaId),
            })
        },
    })
}

export function useUpdateReward() {
    const queryClient = useQueryClient()

    return useMutation({
        mutationFn: ({ id, data }: { id: string; data: UpdateRewardRequest }) =>
            rewardService.update(id, data),
        onSuccess: (reward) => {
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.rewards.byCampania(reward.campaniaId),
            })
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.rewards.byId(reward.id),
            })
        },
    })
}

export function useDeleteReward() {
    const queryClient = useQueryClient()

    return useMutation({
        mutationFn: (id: string) => rewardService.delete(id),
        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.rewards.all,
            })
        },
    })
}

export function useReorderRewards() {
    const queryClient = useQueryClient()

    return useMutation({
        mutationFn: (data: ReorderRewardsRequest) => rewardService.reorder(data),
        onSuccess: (_, variables) => {
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.rewards.byCampania(variables.campaniaId),
            })
        },
    })
}
```

---

## 5. Services

### 5.1 rewardService

**Archivo:** `src/admin/src/services/reward.service.ts`

**Metodos:**
| Metodo | Input | Output | Endpoint |
|--------|-------|--------|----------|
| getAll | - | Reward[] | GET /api/rewards |
| getByCampania | campaniaId: string | Reward[] | GET /api/rewards?campaniaId={id} |
| getById | id: string | Reward | GET /api/rewards/{id} |
| create | data: CreateRewardRequest | Reward | POST /api/rewards |
| update | id: string, data: UpdateRewardRequest | Reward | PUT /api/rewards/{id} |
| delete | id: string | void | DELETE /api/rewards/{id} |
| reorder | data: ReorderRewardsRequest | void | PUT /api/rewards/reorder |

**Implementacion completa:**

```typescript
import { apiFetch } from "@/lib/api-client"
import type {
    Reward,
    CreateRewardRequest,
    UpdateRewardRequest,
    ReorderRewardsRequest,
    ServiceResponse,
} from "@shared/types"
import { API_ROUTES } from "@shared/constants"

class RewardService {
    async getAll(): Promise<Reward[]> {
        const response = await apiFetch<ServiceResponse<Reward[]>>(
            API_ROUTES.rewards.base
        )
        return response.data || []
    }

    async getByCampania(campaniaId: string): Promise<Reward[]> {
        const response = await apiFetch<ServiceResponse<Reward[]>>(
            `${API_ROUTES.rewards.base}?campaniaId=${campaniaId}`
        )
        return response.data || []
    }

    async getById(id: string): Promise<Reward | null> {
        try {
            const response = await apiFetch<ServiceResponse<Reward>>(
                API_ROUTES.rewards.byId(id)
            )
            return response.data || null
        } catch {
            return null
        }
    }

    async create(data: CreateRewardRequest): Promise<Reward> {
        const response = await apiFetch<ServiceResponse<Reward>>(
            API_ROUTES.rewards.base,
            {
                method: "POST",
                data,
            }
        )
        return response.data!
    }

    async update(id: string, data: UpdateRewardRequest): Promise<Reward> {
        const response = await apiFetch<ServiceResponse<Reward>>(
            API_ROUTES.rewards.byId(id),
            {
                method: "PUT",
                data,
            }
        )
        return response.data!
    }

    async delete(id: string): Promise<void> {
        await apiFetch(API_ROUTES.rewards.byId(id), { method: "DELETE" })
    }

    async reorder(data: ReorderRewardsRequest): Promise<void> {
        await apiFetch(API_ROUTES.rewards.reorder, {
            method: "PUT",
            data,
        })
    }
}

export const rewardService = new RewardService()
```

---

## 6. Flujo de Datos

```
User Action (Drag & Drop / Edit / Delete)
    ↓
RewardsListPage (page component)
    ↓
useReorderRewards / useUpdateReward / useDeleteReward (mutation hook)
    ↓
rewardService.reorder() / update() / delete() (service method)
    ↓
apiFetch → API (PUT /api/rewards/reorder | PUT /api/rewards/{id} | DELETE /api/rewards/{id})
    ↓
Backend (ReorderRewardsCommand | UpdateRewardCommand | DeleteRewardCommand)
    ↓
Success → queryClient.invalidateQueries → useRewards refetch → UI update
```

**Flujo de creacion/edicion:**

```
User fills form → Click Guardar
    ↓
RewardFormModal → handleSubmit
    ↓
useCreateReward.mutate() / useUpdateReward.mutate()
    ↓
rewardService.create() / update()
    ↓
POST /api/rewards | PUT /api/rewards/{id}
    ↓
Success → invalidate queries → toast success → modal close → list refresh
```

---

## 7. Dependencias de Shared

**Importar de `@shared/`:**

**Types:**
- `Reward`
- `RewardListItem` (no usado en Admin, pero disponible)
- `CreateRewardRequest`
- `UpdateRewardRequest`
- `ReorderRewardsRequest`
- `RewardOrder`
- `ServiceResponse`
- `TipoReward` (enum)

**Schemas:**
- `createRewardSchema`
- `updateRewardSchema`
- `reorderRewardsSchema`
- `CreateRewardFormData` (type inferred)
- `UpdateRewardFormData` (type inferred)

**Constants:**
- `QUERY_KEYS.rewards.all`
- `QUERY_KEYS.rewards.byId(id)`
- `QUERY_KEYS.rewards.byCampania(campaniaId)`
- `QUERY_KEYS.rewards.filtered(filters)`
- `API_ROUTES.rewards.base`
- `API_ROUTES.rewards.byId(id)`
- `API_ROUTES.rewards.reorder`
- `TIPO_REWARD` (object con IDs)
- `TIPO_REWARD_LABELS` (labels para dropdown)
- `TIPO_REWARD_DESCRIPTIONS` (tooltips opcionales)

**Utils:**
- `getRewardErrorMessage(errorCode)` (opcional, para mensajes custom)
- `getRewardSpecificErrorMessage(errorCode)` (para tooltips/modals)

---

## 8. Dependencias Externas

### Packages a Instalar (si no existen)

```bash
cd src/admin
npm install @dnd-kit/core @dnd-kit/sortable @dnd-kit/utilities
```

### shadcn/ui Components a Agregar (si no existen)

```bash
npx shadcn-ui@latest add alert-dialog
npx shadcn-ui@latest add checkbox
```

**Componentes shadcn ya existentes:**
- `Dialog` ✓
- `Card` ✓
- `Button` ✓
- `Input` ✓
- `Textarea` ✓
- `Select` ✓
- `Label` ✓
- `Badge` ✓

---

## 9. Archivos a Crear

| Archivo | Tipo | Descripcion |
|---------|------|-------------|
| `src/admin/src/app/(dashboard)/campanias/[id]/recompensas/page.tsx` | Page | Pagina principal con lista sortable |
| `src/admin/src/app/(dashboard)/campanias/[id]/recompensas/components/RewardCard.tsx` | Component | Card sortable individual |
| `src/admin/src/app/(dashboard)/campanias/[id]/recompensas/components/RewardFormModal.tsx` | Component | Modal create/edit con form |
| `src/admin/src/app/(dashboard)/campanias/[id]/recompensas/components/RewardDeleteDialog.tsx` | Component | Dialog confirmacion delete |
| `src/admin/src/app/(dashboard)/campanias/[id]/recompensas/components/RewardStatsCard.tsx` | Component | Stats summary card |
| `src/admin/src/hooks/use-rewards.ts` | Hooks | Todos los hooks de rewards |
| `src/admin/src/services/reward.service.ts` | Service | API calls completas |

---

## 10. Integracion con @dnd-kit

### Setup basico

**1. Providers:**
Ya existe `QueryClientProvider` en layout. No se requiere provider adicional para dnd-kit.

**2. Componente sortable:**

```tsx
import { DndContext, DragEndEvent, closestCenter } from "@dnd-kit/core"
import { SortableContext, verticalListSortingStrategy } from "@dnd-kit/sortable"

<DndContext collisionDetection={closestCenter} onDragEnd={handleDragEnd}>
    <SortableContext items={rewards.map((r) => r.id)} strategy={verticalListSortingStrategy}>
        {rewards.map((reward) => (
            <RewardCard key={reward.id} reward={reward} />
        ))}
    </SortableContext>
</DndContext>
```

**3. Item sortable:**

```tsx
import { useSortable } from "@dnd-kit/sortable"
import { CSS } from "@dnd-kit/utilities"

const { attributes, listeners, setNodeRef, transform, transition } = useSortable({ id: reward.id })

const style = {
    transform: CSS.Transform.toString(transform),
    transition,
}

<div ref={setNodeRef} style={style} {...attributes}>
    <div {...listeners}>{/* Drag handle */}</div>
    {/* Resto del contenido */}
</div>
```

---

## 11. Responsive Behavior

### Breakpoints

| Breakpoint | Width | Cambios |
|------------|-------|---------|
| **Mobile** | < 640px | Sidebar colapsado, stats grid 2x2, drag handles permanentes, botones icon-only, modal full-screen |
| **Tablet** | 640-1024px | Sidebar visible, drag handles al hover, layout normal |
| **Desktop** | > 1024px | Layout completo, max-width 5xl, drag handles al hover |

### Clases Tailwind

```tsx
// RewardsListPage
<div className="container max-w-5xl mx-auto py-8 px-4 md:px-6">

// RewardCard
<Button className="text-primary hover:bg-primary/10">
    <Edit className="w-4 h-4 md:mr-2" />
    <span className="hidden md:inline">Editar</span>
</Button>

// RewardFormModal
<DialogContent className="max-w-full md:max-w-2xl">
<div className="grid grid-cols-1 md:grid-cols-2 gap-4">
```

---

## 12. Testing Considerations

### Unit Tests (Vitest)

**Archivos a testear:**
- `RewardCard.test.tsx` - Renderizado, drag handle, botones
- `RewardFormModal.test.tsx` - Validacion Zod, conditional sections
- `RewardDeleteDialog.test.tsx` - Variantes con/sin backings
- `use-rewards.test.ts` - Mutations, query invalidation

**Ejemplo test:**

```typescript
// use-rewards.test.ts
import { renderHook, waitFor } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { useCreateReward } from "./use-rewards"

describe("useCreateReward", () => {
    it("invalidates queries on success", async () => {
        const queryClient = new QueryClient()
        const wrapper = ({ children }) => (
            <QueryClientProvider client={queryClient}>
                {children}
            </QueryClientProvider>
        )

        const { result } = renderHook(() => useCreateReward(), { wrapper })

        // Mock create
        result.current.mutate(mockRewardData)

        await waitFor(() => expect(result.current.isSuccess).toBe(true))
        // Verify invalidation
    })
})
```

---

## 13. Checklist

- [ ] Componentes usan shadcn/ui (Dialog, AlertDialog, Card, Button, Input, Textarea, Select, Checkbox)
- [ ] Hooks siguen patron useQuery/useMutation
- [ ] Services manejan errores correctamente (ServiceResponse)
- [ ] Types importados de @shared
- [ ] Schemas Zod importados de @shared/schemas
- [ ] Formularios usan React Hook Form + zodResolver
- [ ] Drag & drop implementado con @dnd-kit
- [ ] Query invalidation correcta en mutations
- [ ] Toast notifications configuradas (sonner)
- [ ] Responsive design (mobile-first)
- [ ] Character counters en inputs de texto
- [ ] Conditional sections con fade-in animation
- [ ] Loading states en buttons (spinner + texto)
- [ ] Empty state con icono + CTA
- [ ] Delete dialog con variantes (con/sin backings)
- [ ] API routes alineadas con backend (/api/rewards)
- [ ] ErrorCode 4010 manejado especificamente

---

## 14. Siguiente Paso Sugerido

1. **Crear componentes shadcn faltantes:**
   ```bash
   cd src/admin
   npx shadcn-ui@latest add alert-dialog
   npx shadcn-ui@latest add checkbox
   ```

2. **Instalar @dnd-kit:**
   ```bash
   npm install @dnd-kit/core @dnd-kit/sortable @dnd-kit/utilities
   ```

3. **Implementar en orden:**
   - `reward.service.ts` (base para hooks)
   - `use-rewards.ts` (hooks para componentes)
   - `RewardStatsCard.tsx` (componente simple)
   - `RewardCard.tsx` (con drag & drop)
   - `RewardFormModal.tsx` (formulario completo)
   - `RewardDeleteDialog.tsx` (confirmacion)
   - `page.tsx` (orquestacion final)

4. **Verificar integracion con shared:**
   - Asegurar que `plans/definir-recompensas/shared/contracts-plan.md` este implementado primero
   - Verificar imports de types, schemas, constants

---

**Fin del plan frontend Admin.**
