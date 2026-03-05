// ServiceResponse wrapper - matches backend ServiceResponse<T>
export interface ServiceResponse<T> {
  data: T
  messages: ServiceResponseMessage[]
}

export interface ServiceResponseMessage {
  message: string
  errorCode: string
}

// Pagination
export interface PaginatedRequest {
  page?: number
  pageSize?: number
  sortBy?: string
  sortDirection?: "asc" | "desc"
}

export interface PaginatedResponse<T> {
  items: T[]
  totalCount: number
  page: number
  pageSize: number
  totalPages: number
}
