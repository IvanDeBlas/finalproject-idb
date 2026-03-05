import { useMutation } from "@tanstack/react-query"
import { conversacionApi } from "../../infrastructure"
import type { CreateConversacionRequest } from "../../domain"

export function useCreateConversacion() {
    return useMutation({
        mutationFn: (data: CreateConversacionRequest) =>
            conversacionApi.create(data),
    })
}
