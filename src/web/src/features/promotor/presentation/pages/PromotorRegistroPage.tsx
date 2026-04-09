import { Navigate, useNavigate } from "react-router-dom"
import { Card, CardContent } from "@/components/ui/card"
import { Megaphone } from "lucide-react"
import { APP_ROUTES } from "@shared/constants"
import { mapCreateFormToRequest } from "@shared/utils/mappers"
import type { CreatePromotorFormData } from "@shared/schemas/crowdpromotion.schema"
import { usePromotor, useCreatePromotor } from "../../application"
import { PromotorForm } from "../components/PromotorForm"

export default function PromotorRegistroPage() {
    const navigate = useNavigate()
    const { data: promotor, isLoading } = usePromotor()
    const createMutation = useCreatePromotor()

    // Guard: if already has profile, redirect to dashboard
    if (!isLoading && promotor) {
        return <Navigate to={APP_ROUTES.landing.promotor.dashboard} replace />
    }

    // Loading state
    if (isLoading) {
        return (
            <div className="flex h-screen items-center justify-center bg-[#1a1a2e]">
                <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-pink-500" />
            </div>
        )
    }

    // Only show form if error (404 = no profile) or no data
    const handleCreate = async (data: CreatePromotorFormData) => {
        const request = mapCreateFormToRequest(data)
        await createMutation.mutateAsync(request)
    }

    return (
        <div className="min-h-screen bg-[#1a1a2e] py-12 px-4">
            <div className="mx-auto mb-8 text-center">
                <div className="w-14 h-14 p-3 rounded-xl bg-gradient-to-br from-pink-500 to-purple-600 text-white mx-auto mb-4 flex items-center justify-center">
                    <Megaphone className="w-7 h-7" />
                </div>
                <h1 className="text-3xl font-bold text-white mb-2">
                    Conviertete en Promotor
                </h1>
                <p className="text-base text-[#94a3b8] max-w-sm mx-auto">
                    Difunde la musica que amas y gana comisiones
                </p>
            </div>

            <Card className="max-w-lg mx-auto bg-[#151525] border border-[#334155] rounded-xl shadow-lg">
                <CardContent className="p-6 sm:p-8">
                    <PromotorForm
                        mode="create"
                        onSubmit={handleCreate}
                        isSubmitting={createMutation.isPending}
                        onCancel={() => navigate(APP_ROUTES.landing.home)}
                    />
                </CardContent>
            </Card>
        </div>
    )
}
