"use client"

import { useState } from "react"
import { Skeleton } from "@/components/ui/skeleton"
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import { usePromotor } from "@/hooks/use-promotor"
import { PromotorProfileCard } from "./PromotorProfileCard"
import { PromotorEditForm } from "./PromotorEditForm"
import { PromotorNoEncontrado } from "./PromotorNoEncontrado"
import { DesactivarPromotorDialog } from "./DesactivarPromotorDialog"

export function PromotorPageClient() {
    const { data: promotor, isLoading, isError } = usePromotor()
    const [showDesactivarDialog, setShowDesactivarDialog] = useState(false)
    const [activeTab, setActiveTab] = useState("perfil")

    if (isLoading) {
        return (
            <div className="space-y-6">
                <Skeleton className="h-8 w-64" />
                <Skeleton className="h-[400px] rounded-lg" />
            </div>
        )
    }

    if (!promotor || isError) {
        return <PromotorNoEncontrado />
    }

    return (
        <div className="space-y-6">
            <div>
                <h1 className="text-3xl font-bold">Mi Perfil Promotor</h1>
                <p className="text-muted-foreground">
                    Gestiona tu perfil y visibilidad como promotor
                </p>
            </div>

            <Tabs value={activeTab} onValueChange={setActiveTab}>
                <TabsList>
                    <TabsTrigger value="perfil">Perfil</TabsTrigger>
                    <TabsTrigger value="editar">Editar</TabsTrigger>
                </TabsList>

                <TabsContent value="perfil" className="mt-6">
                    <PromotorProfileCard
                        promotor={promotor}
                        onEditClick={() => setActiveTab("editar")}
                        onDesactivarClick={() => setShowDesactivarDialog(true)}
                    />
                </TabsContent>

                <TabsContent value="editar" className="mt-6">
                    <PromotorEditForm
                        promotor={promotor}
                        onSuccess={() => setActiveTab("perfil")}
                        onCancel={() => setActiveTab("perfil")}
                    />
                </TabsContent>
            </Tabs>

            <DesactivarPromotorDialog
                open={showDesactivarDialog}
                onOpenChange={setShowDesactivarDialog}
                totalProgramasActivos={promotor.totalProgramasActivos}
            />
        </div>
    )
}
