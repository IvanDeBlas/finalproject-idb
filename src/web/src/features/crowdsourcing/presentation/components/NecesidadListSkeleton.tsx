import { FC } from "react"
import { Skeleton } from "@/components/ui/skeleton"

export const NecesidadListSkeleton: FC = () => {
    return (
        <div className="space-y-8">
            {[1, 2, 3].map((fase) => (
                <div key={fase}>
                    <Skeleton className="h-6 w-40 mb-4 bg-[#1e2a42]" />
                    {[1, 2].map((item) => (
                        <Skeleton key={item} className="h-24 w-full mb-3 bg-[#1e2a42] rounded-lg" />
                    ))}
                </div>
            ))}
        </div>
    )
}
