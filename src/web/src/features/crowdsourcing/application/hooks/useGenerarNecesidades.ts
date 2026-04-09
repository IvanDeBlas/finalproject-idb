import { useMutation, useQueryClient } from "@tanstack/react-query"
import { crowdsourcingApi } from "../../infrastructure"
import type { GenerarNecesidadesRequest } from "../../domain"

export function useGenerarNecesidades() {
    const queryClient = useQueryClient()

    return useMutation({
        mutationFn: ({ templateId, data }: { templateId: string; data: GenerarNecesidadesRequest }) =>
            crowdsourcingApi.generarNecesidades(templateId, data),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ["crowdsourcing"] })
        },
    })
}
