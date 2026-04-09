import { useMutation, useQueryClient } from "@tanstack/react-query"
import { templateService } from "@/services/template.service"
import type { CreateTemplateRequest, UpdateTemplateRequest } from "@/services/template.service"
import type { GenerarNecesidadesRequest } from "@shared/types"
import { QUERY_KEYS } from "@shared/constants"
import { toast } from "sonner"

export function useCreateTemplate() {
    const queryClient = useQueryClient()

    return useMutation({
        mutationFn: (data: CreateTemplateRequest) => templateService.create(data),
        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.crowdsourcing.templates.all,
            })
            toast.success("Template creado correctamente")
        },
        onError: () => {
            toast.error("Error al crear template")
        },
    })
}

export function useUpdateTemplate() {
    const queryClient = useQueryClient()

    return useMutation({
        mutationFn: ({ id, data }: { id: string; data: UpdateTemplateRequest }) =>
            templateService.update(id, data),
        onSuccess: (_, { id }) => {
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.crowdsourcing.templates.byId(id),
            })
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.crowdsourcing.templates.all,
            })
            toast.success("Template actualizado correctamente")
        },
        onError: () => {
            toast.error("Error al actualizar template")
        },
    })
}

export function useDeleteTemplate() {
    const queryClient = useQueryClient()

    return useMutation({
        mutationFn: (id: string) => templateService.delete(id),
        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.crowdsourcing.templates.all,
            })
            toast.success("Template eliminado correctamente")
        },
        onError: () => {
            toast.error("Error al eliminar template")
        },
    })
}

export function useToggleTemplateStatus() {
    const queryClient = useQueryClient()

    return useMutation({
        mutationFn: (id: string) => templateService.toggleStatus(id),
        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.crowdsourcing.templates.all,
            })
            toast.success("Estado del template actualizado")
        },
        onError: () => {
            toast.error("Error al cambiar estado del template")
        },
    })
}

export function useGenerateNecesidades() {
    return useMutation({
        mutationFn: ({
            templateId,
            data,
        }: {
            templateId: string
            data: GenerarNecesidadesRequest
        }) => templateService.generarNecesidades(templateId, data),
        onSuccess: (result) => {
            toast.success(
                `${result.necesidadesCreadas} necesidades profesionales creadas`
            )
        },
        onError: () => {
            toast.error("Error al generar necesidades desde el template")
        },
    })
}
