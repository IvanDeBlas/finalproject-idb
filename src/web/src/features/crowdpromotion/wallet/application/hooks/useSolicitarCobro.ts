import { useMutation } from "@tanstack/react-query"
import { walletService } from "../../infrastructure/wallet.service"
import type { SolicitarCobroRequest, SolicitarCobroResponse } from "../../domain"

export function useSolicitarCobro() {
    return useMutation<SolicitarCobroResponse, Error & { errorCode?: string }, SolicitarCobroRequest>({
        mutationFn: (data) => walletService.solicitarCobro(data),
    })
}
