import { useQuery } from "@tanstack/react-query"
import { maestrasService } from "@/services/maestras.service"
import { QUERY_KEYS } from "@shared/constants"

export function useRolesProfesionales() {
    return useQuery({
        queryKey: QUERY_KEYS.crowdsourcing.maestras.rolesProfesionales,
        queryFn: () => maestrasService.getRolesProfesionales(),
        staleTime: 10 * 60 * 1000,
    })
}
