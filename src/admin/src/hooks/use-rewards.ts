import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query"
import { rewardService } from "@/services/reward.service"
import { QUERY_KEYS } from "@shared/constants"
import type {
    CreateRewardRequest,
    UpdateRewardRequest,
    ReorderRewardsRequest,
} from "@shared/types"

export function useRewards(campaniaId: string) {
    return useQuery({
        queryKey: QUERY_KEYS.rewards.byCampania(campaniaId),
        queryFn: () => rewardService.getByCampania(campaniaId),
        enabled: !!campaniaId,
    })
}

export function useReward(id: string) {
    return useQuery({
        queryKey: QUERY_KEYS.rewards.byId(id),
        queryFn: () => rewardService.getById(id),
        enabled: !!id,
    })
}

export function useCreateReward() {
    const queryClient = useQueryClient()

    return useMutation({
        mutationFn: (data: CreateRewardRequest) => rewardService.create(data),
        onSuccess: (_, variables) => {
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.rewards.byCampania(variables.campaniaId),
            })
        },
    })
}

export function useUpdateReward() {
    const queryClient = useQueryClient()

    return useMutation({
        mutationFn: ({ id, data }: { id: string; data: UpdateRewardRequest }) =>
            rewardService.update(id, data),
        onSuccess: (result) => {
            if (result) {
                queryClient.invalidateQueries({
                    queryKey: QUERY_KEYS.rewards.byCampania(result.campaniaId),
                })
                queryClient.invalidateQueries({
                    queryKey: QUERY_KEYS.rewards.byId(result.id),
                })
            }
        },
    })
}

export function useDeleteReward() {
    const queryClient = useQueryClient()

    return useMutation({
        mutationFn: (id: string) => rewardService.delete(id),
        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.rewards.all,
            })
        },
    })
}

export function useReorderRewards() {
    const queryClient = useQueryClient()

    return useMutation({
        mutationFn: (data: ReorderRewardsRequest) =>
            rewardService.reorder(data),
        onSuccess: (_, variables) => {
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.rewards.byCampania(variables.campaniaId),
            })
        },
    })
}
