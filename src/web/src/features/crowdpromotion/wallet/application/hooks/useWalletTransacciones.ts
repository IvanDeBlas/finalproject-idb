import { useQuery, keepPreviousData } from "@tanstack/react-query"
import { QUERY_KEYS } from "@shared/constants"
import { walletService } from "../../infrastructure/wallet.service"
import type { WalletTransaccionesFilters } from "../../domain"

export function useWalletTransacciones(filters: WalletTransaccionesFilters) {
    return useQuery({
        queryKey: QUERY_KEYS.crowdpromotion.wallet.transacciones(filters),
        queryFn: () => walletService.getTransacciones(filters),
        staleTime: 30 * 1000,
        retry: 1,
        placeholderData: keepPreviousData,
    })
}
