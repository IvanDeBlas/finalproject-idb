import { useMutation, useQueryClient } from "@tanstack/react-query"
import { useRouter } from "next/navigation"
import { toast } from "sonner"
import { necesidadService } from "@/services/necesidad.service"
import { QUERY_KEYS, APP_ROUTES } from "@shared/constants"
import type {
    CreateNecesidadRequest,
    UpdateNecesidadRequest,
    CerrarNecesidadRequest,
    NecesidadCreateResult,
    NecesidadUpdateResult,
    CerrarNecesidadResult,
} from "@shared/types"

export function useCreateNecesidad() {
    const queryClient = useQueryClient()
    const router = useRouter()

    return useMutation<NecesidadCreateResult, Error, CreateNecesidadRequest>({
        mutationFn: (data) => necesidadService.create(data),
        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.crowdsourcing.necesidades.mis,
            })
            toast.success("Necesidad publicada correctamente")
            router.push(APP_ROUTES.dashboard.crowdsourcing.necesidades)
        },
        onError: (error) => {
            toast.error(error.message || "Error al publicar la necesidad")
        },
    })
}

export function useUpdateNecesidad(id: string) {
    const queryClient = useQueryClient()
    const router = useRouter()

    return useMutation<NecesidadUpdateResult, Error, UpdateNecesidadRequest>({
        mutationFn: (data) => necesidadService.update(id, data),
        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.crowdsourcing.necesidades.mis,
            })
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.crowdsourcing.necesidades.byId(id),
            })
            toast.success("Necesidad actualizada correctamente")
            router.push(APP_ROUTES.dashboard.crowdsourcing.necesidadDetail(id))
        },
        onError: (error) => {
            toast.error(error.message || "Error al actualizar la necesidad")
        },
    })
}

export function useCerrarNecesidad(id: string) {
    const queryClient = useQueryClient()

    return useMutation<CerrarNecesidadResult, Error, CerrarNecesidadRequest>({
        mutationFn: (data) => necesidadService.cerrar(id, data),
        onSuccess: (result) => {
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.crowdsourcing.necesidades.mis,
            })
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.crowdsourcing.necesidades.byId(id),
            })

            const message = result.propuestasRechazadas > 0
                ? `Necesidad cerrada. Se han rechazado ${result.propuestasRechazadas} propuestas pendientes.`
                : "Necesidad cerrada correctamente"

            toast.success(message)
        },
        onError: (error) => {
            toast.error(error.message || "Error al cerrar la necesidad")
        },
    })
}
