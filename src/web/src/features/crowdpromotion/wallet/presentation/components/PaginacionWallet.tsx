import type { FC } from "react"
import {
    Pagination,
    PaginationContent,
    PaginationEllipsis,
    PaginationItem,
    PaginationLink,
    PaginationNext,
    PaginationPrevious,
} from "@/components/ui/pagination"

interface PaginacionWalletProps {
    page: number
    totalPages: number
    totalCount: number
    pageSize: number
    onPageChange: (page: number) => void
}

function getPageNumbers(current: number, total: number): (number | "ellipsis")[] {
    if (total <= 5) {
        return Array.from({ length: total }, (_, i) => i + 1)
    }

    const pages: (number | "ellipsis")[] = [1]

    if (current > 3) {
        pages.push("ellipsis")
    }

    const start = Math.max(2, current - 1)
    const end = Math.min(total - 1, current + 1)

    for (let i = start; i <= end; i++) {
        pages.push(i)
    }

    if (current < total - 2) {
        pages.push("ellipsis")
    }

    pages.push(total)

    return pages
}

export const PaginacionWallet: FC<PaginacionWalletProps> = ({
    page,
    totalPages,
    totalCount,
    pageSize,
    onPageChange,
}) => {
    if (totalPages <= 1) return null

    const startRecord = (page - 1) * pageSize + 1
    const endRecord = Math.min(page * pageSize, totalCount)
    const pageNumbers = getPageNumbers(page, totalPages)

    return (
        <div className="flex flex-col items-center gap-3 sm:flex-row sm:justify-between">
            <p className="text-sm text-[#94a3b8]">
                Mostrando {startRecord}-{endRecord} de {totalCount} transacciones
            </p>

            <Pagination>
                <PaginationContent>
                    <PaginationItem>
                        <PaginationPrevious
                            onClick={() => onPageChange(page - 1)}
                            className={page <= 1 ? "opacity-40 pointer-events-none" : "cursor-pointer"}
                        />
                    </PaginationItem>

                    {pageNumbers.map((p, i) =>
                        p === "ellipsis" ? (
                            <PaginationItem key={`ellipsis-${i}`}>
                                <PaginationEllipsis />
                            </PaginationItem>
                        ) : (
                            <PaginationItem key={p}>
                                <PaginationLink
                                    isActive={p === page}
                                    onClick={() => onPageChange(p)}
                                    className={`cursor-pointer ${
                                        p === page ? "bg-[#a855f7] text-white border-[#a855f7]" : ""
                                    }`}
                                >
                                    {p}
                                </PaginationLink>
                            </PaginationItem>
                        )
                    )}

                    <PaginationItem>
                        <PaginationNext
                            onClick={() => onPageChange(page + 1)}
                            className={page >= totalPages ? "opacity-40 pointer-events-none" : "cursor-pointer"}
                        />
                    </PaginationItem>
                </PaginationContent>
            </Pagination>
        </div>
    )
}
