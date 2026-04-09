import { FC } from "react"
import { Card } from "@/components/ui/card"
import { Skeleton } from "@/components/ui/skeleton"

interface PropuestaCardSkeletonProps {
    className?: string
}

export const PropuestaCardSkeleton: FC<PropuestaCardSkeletonProps> = ({
    className = "",
}) => {
    return (
        <Card className={`bg-[#0f1729] border-[#334155] rounded-xl p-5 ${className}`}>
            <div role="status" aria-label="Cargando propuesta...">
                <div className="flex items-start justify-between mb-3">
                    <div className="flex-1">
                        <Skeleton className="h-5 w-3/4 mb-2" />
                        <Skeleton className="h-4 w-40" />
                    </div>
                    <Skeleton className="h-6 w-24 rounded-full flex-shrink-0" />
                </div>
                <div className="space-y-2 mb-4">
                    <Skeleton className="h-4 w-28" />
                    <Skeleton className="h-4 w-36" />
                </div>
                <div className="flex justify-end">
                    <Skeleton className="h-8 w-32" />
                </div>
            </div>
        </Card>
    )
}
