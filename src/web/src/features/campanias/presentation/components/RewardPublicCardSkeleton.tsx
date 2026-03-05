import { Card } from "@/components/ui/card"
import { Skeleton } from "@/components/ui/skeleton"

interface RewardPublicCardSkeletonProps {
    count?: number
    className?: string
}

export function RewardPublicCardSkeleton({
    count = 3,
}: RewardPublicCardSkeletonProps) {
    return (
        <>
            {Array.from({ length: count }).map((_, i) => (
                <Card
                    key={i}
                    className="bg-[#1a1a2e] border-[#334155] p-3 sm:p-4 mb-2 sm:mb-3"
                    data-testid="reward-skeleton"
                >
                    <div className="flex items-start justify-between mb-2">
                        <Skeleton className="h-7 w-20 bg-[#334155]" />
                        <Skeleton className="h-5 w-20 bg-[#334155]" />
                    </div>
                    <Skeleton className="h-5 w-3/4 bg-[#334155] mb-2" />
                    <Skeleton className="h-4 w-full bg-[#334155] mb-1" />
                    <Skeleton className="h-4 w-full bg-[#334155] mb-2" />
                    <Skeleton className="h-4 w-1/2 bg-[#334155] mb-3" />
                    <Skeleton className="h-9 w-full bg-[#334155]" />
                </Card>
            ))}
        </>
    )
}
