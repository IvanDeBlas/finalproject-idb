// Import from shared contracts
import type {
    Backing,
    BackingDto,
    BackingPublicDto,
    CreateBackingRequest,
    CampaniaStats,
} from "@shared/types/backing"

// Re-export for feature consumers
export type {
    Backing,
    BackingDto,
    BackingPublicDto,
    CreateBackingRequest,
    CampaniaStats,
}

export interface IBackingRepository {
    create(data: CreateBackingRequest & { campaniaId: string }): Promise<BackingDto>
    getByCampaniaId(campaniaId: string, options?: PaginationParams): Promise<BackingPublicDto[]>
    getMyBackings(): Promise<BackingDto[]>
}

export interface PaginationParams {
    pageNumber?: number
    pageSize?: number
}
