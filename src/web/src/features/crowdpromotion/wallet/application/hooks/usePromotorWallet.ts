import { useQuery } from "@tanstack/react-query"
import { QUERY_KEYS } from "@shared/constants"
import { walletService } from "../../infrastructure/wallet.service"

export function usePromotorWallet() {
    return useQuery({
        queryKey: QUERY_KEYS.crowdpromotion.wallet.resumen,
        queryFn: () => walletService.getWalletResumen(),
        staleTime: 30 * 1000,
        retry: 1,
    })
}
