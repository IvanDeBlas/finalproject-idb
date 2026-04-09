import { Card } from "@/components/ui/card"
import { Skeleton } from "@/components/ui/skeleton"

interface CampaniaListSkeletonProps {
    count?: number
}

export function CampaniaListSkeleton({ count = 6 }: CampaniaListSkeletonProps) {
    return (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {[...Array(count)].map((_, i) => (
                <Card key={i} className="bg-[#0f1729] border-[#334155] p-0 overflow-hidden">
                    <Skeleton className="w-full h-48 rounded-none" />
                    <div className="p-4 space-y-3">
                        {/* Badge */}
                        <Skeleton className="h-5 w-16 rounded-full" />
                        {/* Title */}
                        <Skeleton className="h-6 w-3/4" />
                        {/* Artist */}
                        <div className="flex items-center gap-2">
                            <Skeleton className="w-5 h-5 rounded-full" />
                            <Skeleton className="h-4 w-24" />
                        </div>
                        {/* Progress bar */}
                        <Skeleton className="h-2 w-full" />
                        {/* Stats row */}
                        <div className="flex justify-between">
                            <Skeleton className="h-4 w-20" />
                            <Skeleton className="h-4 w-16" />
                        </div>
                        {/* Days remaining */}
                        <Skeleton className="h-3 w-28" />
                    </div>
                </Card>
            ))}
        </div>
    )
}
