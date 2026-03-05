"use client"

import { useState } from "react"
import { useParams, useRouter } from "next/navigation"
import {
    DndContext,
    type DragEndEvent,
    closestCenter,
    KeyboardSensor,
    PointerSensor,
    useSensor,
    useSensors,
} from "@dnd-kit/core"
import {
    SortableContext,
    verticalListSortingStrategy,
    arrayMove,
    sortableKeyboardCoordinates,
} from "@dnd-kit/sortable"
import { Plus, Lightbulb, ArrowLeft, PackageOpen } from "lucide-react"
import { Button, Skeleton } from "@/components/ui"
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
    const { mutate: reorder } = useReorderRewards()

    const [selectedReward, setSelectedReward] = useState<Reward | null>(null)
    const [rewardToDelete, setRewardToDelete] = useState<Reward | null>(null)
    const [isFormModalOpen, setIsFormModalOpen] = useState(false)
    const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false)

    const sensors = useSensors(
        useSensor(PointerSensor),
        useSensor(KeyboardSensor, {
            coordinateGetter: sortableKeyboardCoordinates,
        })
    )

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
                    onClick={() =>
                        router.push(`/dashboard/campanias/${campaniaId}`)
                    }
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

            {/* Loading State */}
            {isLoading && (
                <div className="space-y-3">
                    {Array.from({ length: 3 }).map((_, i) => (
                        <Skeleton
                            key={i}
                            className="h-[140px] w-full rounded-lg"
                        />
                    ))}
                </div>
            )}

            {/* Stats Card */}
            {!isLoading && rewards.length > 0 && (
                <RewardStatsCard rewards={rewards} />
            )}

            {/* Empty State */}
            {!isLoading && rewards.length === 0 && (
                <EmptyState
                    icon={<PackageOpen className="w-20 h-20" />}
                    title="No has creado recompensas aun"
                    description="Las recompensas incentivan a tus fans a apoyarte. Crea tu primera recompensa ahora."
                    actionLabel="Crear primera recompensa"
                    onAction={handleCreateClick}
                />
            )}

            {/* Rewards List (DnD) */}
            {!isLoading && rewards.length > 0 && (
                <>
                    <DndContext
                        sensors={sensors}
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
                                        onEdit={() =>
                                            handleEditClick(reward)
                                        }
                                        onDelete={() =>
                                            handleDeleteClick(reward)
                                        }
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
            {!isLoading && rewards.length > 0 && (
                <div className="bg-card border-l-4 border-primary p-4 rounded-r-lg">
                    <div className="flex items-start gap-3">
                        <Lightbulb className="w-5 h-5 text-primary mt-0.5" />
                        <div>
                            <p className="text-sm text-muted-foreground">
                                <strong>Tip:</strong> Ordena tus recompensas
                                arrastrandolas. El orden se reflejara en la
                                vista publica.
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
