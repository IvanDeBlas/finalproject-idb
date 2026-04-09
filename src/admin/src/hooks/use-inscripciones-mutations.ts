import { useMutation, useQueryClient } from "@tanstack/react-query"
import { inscripcionService } from "@/services/inscripcion.service"
import { QUERY_KEYS } from "@shared/constants"
import { toast } from "sonner"

interface InscripcionMutationParams {
    programaId: string
    inscripcionId: string
    promotorNombre?: string
}

export function useAprobarInscripcion() {
    const queryClient = useQueryClient()

    return useMutation({
        mutationFn: (params: InscripcionMutationParams) =>
            inscripcionService.aprobar({
                programaId: params.programaId,
                inscripcionId: params.inscripcionId,
            }),
        onSuccess: (data, variables) => {
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.crowdpromotion.programas.inscripciones(
                    variables.programaId,
                    { estado: "Pendiente" }
                ),
            })
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.crowdpromotion.programas.inscripciones(
                    variables.programaId,
                    { estado: "Aprobado" }
                ),
            })
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.crowdpromotion.programas.byId(variables.programaId),
            })
            toast.success(
                `${data.promotorNombre ?? variables.promotorNombre ?? "Promotor"} ha sido aprobado. Se ha generado su codigo referido.`
            )
        },
        onError: (error: Error) => {
            toast.error(error.message || "No se pudo aprobar al promotor. Intentalo de nuevo.")
        },
    })
}

export function useRechazarInscripcion() {
    const queryClient = useQueryClient()

    return useMutation({
        mutationFn: (params: InscripcionMutationParams) =>
            inscripcionService.rechazar({
                programaId: params.programaId,
                inscripcionId: params.inscripcionId,
            }),
        onSuccess: (_data, variables) => {
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.crowdpromotion.programas.inscripciones(
                    variables.programaId,
                    { estado: "Pendiente" }
                ),
            })
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.crowdpromotion.programas.byId(variables.programaId),
            })
            toast(`Solicitud de ${variables.promotorNombre ?? "promotor"} rechazada.`)
        },
        onError: (error: Error) => {
            toast.error(error.message || "No se pudo rechazar la solicitud. Intentalo de nuevo.")
        },
    })
}

export function useBloquearInscripcion() {
    const queryClient = useQueryClient()

    return useMutation({
        mutationFn: (params: InscripcionMutationParams) =>
            inscripcionService.bloquear({
                programaId: params.programaId,
                inscripcionId: params.inscripcionId,
            }),
        onSuccess: (_data, variables) => {
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.crowdpromotion.programas.inscripciones(
                    variables.programaId,
                    { estado: "Pendiente" }
                ),
            })
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.crowdpromotion.programas.inscripciones(
                    variables.programaId,
                    { estado: "Bloqueado" }
                ),
            })
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.crowdpromotion.programas.byId(variables.programaId),
            })
            toast(`${variables.promotorNombre ?? "Promotor"} ha sido bloqueado en este programa.`)
        },
        onError: (error: Error) => {
            toast.error(error.message || "No se pudo bloquear al promotor. Intentalo de nuevo.")
        },
    })
}

export function useDarDeBajaInscripcion() {
    const queryClient = useQueryClient()

    return useMutation({
        mutationFn: (params: InscripcionMutationParams) =>
            inscripcionService.darDeBaja({
                programaId: params.programaId,
                inscripcionId: params.inscripcionId,
            }),
        onSuccess: (_data, variables) => {
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.crowdpromotion.programas.inscripciones(
                    variables.programaId,
                    { estado: "Aprobado" }
                ),
            })
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.crowdpromotion.programas.byId(variables.programaId),
            })
            toast(
                `${variables.promotorNombre ?? "Promotor"} ha sido dado de baja. Su codigo referido ha sido desactivado.`
            )
        },
        onError: (error: Error) => {
            toast.error(error.message || "No se pudo dar de baja al promotor. Intentalo de nuevo.")
        },
    })
}
