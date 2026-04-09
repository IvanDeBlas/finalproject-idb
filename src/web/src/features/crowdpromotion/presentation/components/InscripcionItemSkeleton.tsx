import type { FC } from "react"
import { Skeleton } from "@/components/ui/skeleton"

export const InscripcionItemSkeleton: FC = () => {
    return <Skeleton className="bg-[#1e1e38] rounded-xl h-[160px]" />
}
