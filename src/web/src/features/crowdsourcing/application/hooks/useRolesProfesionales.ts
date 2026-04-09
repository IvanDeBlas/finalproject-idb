import { useQuery } from "@tanstack/react-query"
import { QUERY_KEYS } from "@shared/constants"
import { crowdsourcingApi } from "../../infrastructure"

export function useRolesProfesionales() {
    return useQuery({
        queryKey: QUERY_KEYS.crowdsourcing.maestras.rolesProfesionales,
        queryFn: () => crowdsourcingApi.getRolesProfesionales(),
        staleTime: 30 * 60 * 1000,
    })
}
