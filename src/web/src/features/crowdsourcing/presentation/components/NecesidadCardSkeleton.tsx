import { FC } from "react"
import { Card } from "@/components/ui/card"
import { Skeleton } from "@/components/ui/skeleton"

interface NecesidadCardSkeletonProps {
    className?: string
}

export const NecesidadCardSkeleton: FC<NecesidadCardSkeletonProps> = ({
    className = "",
}) => {
    return (
        <Card className={`bg-[#0f1729] border-[#334155] rounded-xl p-5 ${className}`}>
            <div role="status" aria-label="Cargando necesidad...">
                <div className="flex items-center gap-2 mb-3">
                    <Skeleton className="h-5 w-28 rounded-full" />
                    <Skeleton className="h-5 w-20 rounded-full" />
                </div>
                <Skeleton className="h-6 w-4/5 mb-2" />
                <Skeleton className="h-4 w-full mb-1" />
                <Skeleton className="h-4 w-3/4 mb-4" />
                <div className="flex justify-between mb-3">
                    <Skeleton className="h-4 w-32" />
                    <Skeleton className="h-4 w-28" />
                </div>
                <div className="flex gap-4">
                    <Skeleton className="h-3 w-24" />
                    <Skeleton className="h-3 w-24" />
                </div>
            </div>
        </Card>
    )
}
