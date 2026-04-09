import { useState } from "react"
import { Navigate, useNavigate } from "react-router-dom"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { AlertTriangle } from "lucide-react"
import { APP_ROUTES } from "@shared/constants"
import { mapUpdateFormToRequest, mapPromotorToUpdateForm } from "@shared/utils/mappers"
import type { CreatePromotorFormData } from "@shared/schemas/crowdpromotion.schema"
import { usePromotor, useUpdatePromotor, useDesactivarPromotor } from "../../application"
import { PromotorForm } from "../components/PromotorForm"
import { PromotorDeactivateDialog } from "../components/PromotorDeactivateDialog"

export default function PromotorPerfilPage() {
    const navigate = useNavigate()
    const { data: promotor, isLoading, isError } = usePromotor()
    const updateMutation = useUpdatePromotor()
    const desactivarMutation = useDesactivarPromotor()
    const [isDeactivateDialogOpen, setIsDeactivateDialogOpen] = useState(false)

    // Guard: no profile -> redirect to registration
    if (!isLoading && isError) {
        return <Navigate to={APP_ROUTES.landing.promotor.registro} replace />
    }

    // Loading state
    if (isLoading) {
        return (
            <div className="p-6 max-w-2xl bg-[#1a1a2e] min-h-screen">
                <div className="space-y-6">
                    <div className="h-8 w-48 bg-[#1e1e38] rounded animate-pulse" />
                    <div className="h-4 w-64 bg-[#1e1e38] rounded animate-pulse" />
                    <div className="h-96 bg-[#1e1e38] rounded animate-pulse" />
                </div>
            </div>
        )
    }

    if (!promotor) return null

    const handleUpdate = async (data: CreatePromotorFormData) => {
        const { tipoPromotorId: _, ...updateData } = data
        const request = mapUpdateFormToRequest(updateData)
        await updateMutation.mutateAsync(request)
    }

    const handleDeactivate = async () => {
        await desactivarMutation.mutateAsync()
        setIsDeactivateDialogOpen(false)
    }

    const formDefaults = mapPromotorToUpdateForm(promotor)

    return (
        <div className="p-6 max-w-2xl bg-[#1a1a2e] min-h-screen">
            <h1 className="text-2xl font-bold text-white mb-1">Editar Perfil</h1>
            <p className="text-sm text-[#94a3b8] mb-6">Actualiza tu informacion de promotor</p>

            <div className="space-y-6">
                {/* Form Card */}
                <Card className="bg-[#151525] border-[#334155]">
                    <CardHeader className="border-b border-[#334155] pb-4">
                        <CardTitle className="text-lg font-semibold text-white">
                            Informacion publica
                        </CardTitle>
                    </CardHeader>
                    <CardContent className="pt-6">
                        <PromotorForm
                            mode="edit"
                            defaultValues={formDefaults}
                            tipoPromotorNombre={promotor.tipoPromotorNombre}
                            onSubmit={handleUpdate}
                            isSubmitting={updateMutation.isPending}
                            onCancel={() => navigate(APP_ROUTES.landing.promotor.dashboard)}
                        />
                    </CardContent>
                </Card>

                {/* Danger Zone */}
                <Card className="bg-[#151525] border border-red-900/50">
                    <CardHeader className="border-b border-red-900/50 pb-4">
                        <CardTitle className="text-base font-semibold text-red-400 flex items-center gap-2">
                            <AlertTriangle className="w-4 h-4" />
                            Zona de peligro
                        </CardTitle>
                    </CardHeader>
                    <CardContent className="pt-4">
                        <p className="text-sm text-[#94a3b8] mb-4">
                            Al desactivar tu cuenta de promotor, tu perfil dejara de estar
                            visible y tus programas activos seran dados de baja.
                        </p>
                        <Button
                            variant="outline"
                            className="border-red-900/50 text-red-400 hover:bg-red-950/30 hover:text-red-300"
                            onClick={() => setIsDeactivateDialogOpen(true)}
                        >
                            Desactivar cuenta
                        </Button>
                    </CardContent>
                </Card>
            </div>

            {/* Deactivation Dialog */}
            <PromotorDeactivateDialog
                open={isDeactivateDialogOpen}
                onOpenChange={setIsDeactivateDialogOpen}
                onConfirm={handleDeactivate}
                isConfirming={desactivarMutation.isPending}
                totalProgramasActivos={promotor.totalProgramasActivos}
            />
        </div>
    )
}
